
using CatalogService.App.Core.Domain;
using Microsoft.Data.SqlClient;
using Polly;
using System.Globalization;
using System.IO.Compression;

namespace CatalogService.App.Infrastructure.Context
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger) // Veritabanı tohumlama işlemini başlatan asenkron ana metot.
        {
            // --- 1. Geçici Hata Dayanıklılığı (Transient Fault Handling) için Polly Tanımlaması ---
            var policy = Policy.Handle<SqlException>() // Polly'ye, SqlException (SQL Server'dan kaynaklanan veritabanı hataları) türündeki hataları yakalamasını söyler.
                                                       // (Sizin yorumunuz: sq hatası oluşursa)
                .WaitAndRetryAsync( // Yakalanan hata durumunda bekleyip yeniden deneme stratejisini tanımlar.
                    retryCount: 3, // Maksimum 3 kez yeniden deneme yapılacağını belirtir.
                                   // Yeniden denemeden önce ne kadar bekleneceğini tanımlar (burada her seferinde 5 saniye beklenir).
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) => // Yeniden deneme gerçekleştiğinde çalışacak olay işleyicisini tanımlar.
                    {
                        // Yeniden deneme girişimi, yakalanan hata tipi ve mesajı hakkında uyarı (Warning) seviyesinde log kaydı oluşturur.
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(logger), exception.GetType().Name, exception.Message, retry, 3);
                    }
                );

            // --- 2. Dosya Yollarının Tanımlanması ---
            // Tohumlama verilerini içeren dosyaların bulunduğu dizin yolunu oluşturur (örneğin: 'wwwroot/Infrastructure/Setup/SeedFiles').
            var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");

            // Katalog öğelerinin resimlerinin kopyalanacağı hedef dizin adını tanımlar (göreceli yol).
            var picturePath = "Pics";

            // --- 3. Tohumlama İşleminin Çalıştırılması ---
            // Tanımlanan Polly politikası dahilinde (geçici hata olursa 3 kez 5 saniye arayla yeniden deneyerek) tohumlama metodunu asenkron olarak çalıştırır.
            await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath, logger));
        }


        private async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath, ILogger logger) // Asenkron olarak veritabanı tohumlama işlemini başlatan metot.
        {
            // --- 1. Katalog Markalarını Tohumlama ---
            if (!context.CatalogBrands.Any()) // Veritabanında hiç CatalogBrand (Katalog Markası) kaydı yoksa...
            {
                // Dosyadan okunan tüm marka varlıklarını EF Core takipçisine asenkron olarak ekler.
                await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));

                await context.SaveChangesAsync(); // Değişiklikleri veritabanına kalıcı olarak kaydeder.
            }

            // --- 2. Katalog Tiplerini Tohumlama ---
            if (!context.CatalogTypes.Any()) // Veritabanında hiç CatalogType (Katalog Tipi) kaydı yoksa...
            {
                // Dosyadan okunan tüm tip varlıklarını EF Core takipçisine asenkron olarak ekler.
                await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));

                await context.SaveChangesAsync(); // Değişiklikleri veritabanına kalıcı olarak kaydeder.
            }

            // --- 3. Katalog Öğelerini ve Resimlerini Tohumlama ---
            if (!context.CatalogItems.Any()) // Veritabanında hiç CatalogItem (Katalog Öğesi) kaydı yoksa...
            {
                // Dosyadan okunan tüm ürün varlıklarını (önceki adımlarda eklenen marka ve tip Id'lerini kullanarak) asenkron olarak ekler.
                await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath, context));

                await context.SaveChangesAsync(); // Ürün kayıtlarını veritabanına kaydeder.

                // Ürünlerle ilişkili resim dosyalarını kaynak yoldan hedef yola kopyalar.
                GetCatalogItemPictures(setupDirPath, picturePath);
            }
        }

        private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentPath) // Marka (CatalogBrand) listesini bir dosyadan okuyarak döndüren özel metot.
        {
            // --- Varsayılan Markaları Tanımlayan İç Metot ---
            IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands() // Dosya okuma başarısız olursa kullanılacak varsayılan marka listesini döndürür.
            {
                return new List<CatalogBrand>() // Yeni bir CatalogBrand listesi oluşturur.
        {
            new CatalogBrand() { Brand = "Azure"}, // Varsayılan "Azure" markasını ekler.
            new CatalogBrand() { Brand = ".NET" }, // Varsayılan ".NET" markasını ekler.
            new CatalogBrand() { Brand = "Visual Studio" }, // Varsayılan "Visual Studio" markasını ekler.
            new CatalogBrand() { Brand = "SQL Server" }, // Varsayılan "SQL Server" markasını ekler.
            new CatalogBrand() { Brand = "Other" } // Varsayılan "Other" (Diğer) markasını ekler.
        };
            }

            // --- Dosya Yolunu Oluşturma ve Kontrol ---
            string fileName = Path.Combine(contentPath, "BrandsTextFile.txt"); // Marka verilerini içeren dosyanın tam yolunu oluşturur.

            if (!File.Exists(fileName)) // Eğer belirtilen yolda marka dosyası mevcut değilse...
            {
                return GetPreconfiguredCatalogBrands(); // ...varsayılan (önceden yapılandırılmış) marka listesini hemen döndürür.
            }

            // --- Dosyayı Okuma ve Dönüştürme ---
            var fileContent = File.ReadAllLines(fileName); // Dosyadaki tüm satırları okur ve bir diziye/koleksiyona kaydeder (her satır bir marka adıdır).

            var list = fileContent.Select(i => new CatalogBrand() // Okunan her satırı (i) bir CatalogBrand nesnesine dönüştürmeye başlar.
            {
                Brand = i.Trim('"').Trim() // Satırdaki marka adını alır, varsa çift tırnakları ve gereksiz boşlukları temizler.
            }).Where(i => i != null); // Null (boş) CatalogBrand nesnelerini filtreler (Bu .Select() sonrası teorik olarak gereksizdir, ancak güvenlik için tutulabilir).

            // --- Sonuç Döndürme ---
            // Eğer okunan liste geçerliyse (null değilse) onu döndürür, aksi takdirde varsayılan listeyi döndürür.
            return list ?? GetPreconfiguredCatalogBrands();
        }


        private IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentPath) // Tip (CatalogType) listesini bir dosyadan okuyarak döndüren özel metot.
        {
            // --- Varsayılan Tipleri Tanımlayan İç Metot ---
            IEnumerable<CatalogType> GetPreconfiguredCatalogTypes() // Dosya okuma başarısız olursa kullanılacak varsayılan tip listesini döndürür.
            {
                return new List<CatalogType>() // Yeni bir CatalogType listesi oluşturur.
        {
            new CatalogType() { Type = "Mug"}, // Varsayılan "Mug" tipini ekler.
            new CatalogType() { Type = "T-Shirt" }, // Varsayılan "T-Shirt" tipini ekler.
            new CatalogType() { Type = "Sheet" }, // Varsayılan "Sheet" (Sayfa/Levha) tipini ekler.
            new CatalogType() { Type = "USB Memory Stick" } // Varsayılan "USB Memory Stick" tipini ekler.
        };
            }

            // --- Dosya Yolunu Oluşturma ve Kontrol ---
            string fileName = Path.Combine(contentPath, "CatalogTypes.txt"); // Tip verilerini içeren dosyanın tam yolunu oluşturur.

            if (!File.Exists(fileName)) // Eğer belirtilen yolda tip dosyası mevcut değilse...
            {
                return GetPreconfiguredCatalogTypes(); // ...varsayılan (önceden yapılandırılmış) tip listesini hemen döndürür.
            }

            // --- Dosyayı Okuma ve Dönüştürme ---
            var fileContent = File.ReadAllLines(fileName); // Dosyadaki tüm satırları okur ve bir diziye/koleksiyona kaydeder (her satır bir tip adıdır).

            var list = fileContent.Select(i => new CatalogType() // Okunan her satırı (i) bir CatalogType nesnesine dönüştürmeye başlar.
            {
                Type = i.Trim('"').Trim() // Satırdaki tip adını alır, varsa çift tırnakları ve gereksiz boşlukları temizler.
            }).Where(i => i != null); // Null (boş) CatalogType nesnelerini filtreler.

            // --- Sonuç Döndürme ---
            // Eğer okunan liste geçerliyse (null değilse) onu döndürür, aksi takdirde varsayılan listeyi döndürür.
            return list ?? GetPreconfiguredCatalogTypes();
        }


        private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentPath, CatalogContext context)
        {
            IEnumerable<CatalogItem> GetPreconfiguredItems()
            {
                return new List<CatalogItem>()
                {
                    new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, PictureFileName = "1.png" },
                    new CatalogItem { CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureFileName = "2.png" },
                    new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureFileName = "3.png" },
                    new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, PictureFileName = "4.png" },
                    new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureFileName = "5.png" },
                    new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, PictureFileName = "6.png" },
                    new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureFileName = "7.png" },
                    new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, PictureFileName = "8.png" },
                    new CatalogItem { CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 100, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureFileName = "9.png" },
                    new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureFileName = "10.png" },
                    new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureFileName = "11.png" },
                    new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureFileName = "12.png" },
                };
            }

            string fileName = Path.Combine(contentPath, "CatalogItems.txt"); // Verilerin bulunduğu "CatalogItems.txt" dosyasının tam yolunu oluşturur.

            if (!File.Exists(fileName)) // Eğer belirtilen yolda (contentPath içinde) dosya yoksa...
            {
                return GetPreconfiguredItems(); // ...önceden tanımlanmış (hardcoded) varsayılan öğeleri döndürür.
            }

            // Veritabanından Tip (Type) adlarını ve karşılık gelen Id'lerini bir sözlüğe (Dictionary) yükler.
            var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);

            // Veritabanından Marka (Brand) adlarını ve karşılık gelen Id'lerini bir sözlüğe (Dictionary) yükler.
            var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);


            var fileContent = File.ReadAllLines(fileName) // Belirtilen dosyadaki tüm satırları okur ve bir koleksiyon oluşturur.
                                                          // (Sizin yorumunuz: git ve items dosyasını oku)
                            .Skip(1) // Dosyanın ilk satırını (genellikle başlık/header satırıdır) atlar.
                            .Select(i => i.Split(',')) // Kalan her satırı virgüle (',') göre ayırarak bir dizi (string[]) haline getirir.
                            .Select(i => new CatalogItem() // Her dizi öğesini (satır verisini) yeni bir CatalogItem nesnesine dönüştürmeye başlar.
                            {
                                // İlk sütundaki Tip adını okur, boşlukları temizler ve yukarıdaki sözlükten Id'sini bulup atar.
                                CatalogTypeId = catalogTypeIdLookup[i[0].Trim()],

                                // İkinci sütundaki Marka adını okur, boşlukları temizler ve yukarıdaki sözlükten Id'sini bulup atar.
                                CatalogBrandId = catalogBrandIdLookup[i[1].Trim()],

                                // Üçüncü sütundaki Açıklama (Description) verisini okur ve çift tırnakları/boşlukları temizler.
                                Description = i[2].Trim('"').Trim(),

                                // Dördüncü sütundaki Ad (Name) verisini okur ve çift tırnakları/boşlukları temizler.
                                Name = i[3].Trim('"').Trim(),

                                // Beşinci sütundaki Fiyat (Price) verisini okur, temizler ve ondalık nokta kurallarına uygun olarak Decimal tipine çevirir.
                                Price = Decimal.Parse(i[4].Trim('"').Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),

                                // Altıncı sütundaki Resim Dosya Adı (PictureFileName) verisini okur ve temizler.
                                PictureFileName = i[5].Trim('"').Trim(),

                                // Yedinci sütundaki Stok Miktarı (AvailableStock) verisini okur. Boşsa 0, değilse tamsayıya çevirir.
                                AvailableStock = string.IsNullOrEmpty(i[6].Trim()) ? 0 : int.Parse(i[6].Trim()),

                                // Sekizinci sütundaki Yeniden Siparişte (OnReorder) verisini okur ve Boolean tipine çevirir.
                                OnReorder = Convert.ToBoolean(i[7].Trim())
                            });

            return fileContent; // Oluşturulan CatalogItem nesneleri koleksiyonunu döndürür.
        }

        private void GetCatalogItemPictures(string contentPath, string picturePath) // Katalog öğesi resimlerini hazırlayan ve belirlenen yola kopyalayan metot.
        {
            picturePath ??= "pics"; // Eğer picturePath parametresi null ise, varsayılan değer olarak "pics" dizesini atar (C# 8.0 null birleştirme ataması).

            if (picturePath != null) // picturePath null değilse (ki yukarıdaki satır bunu neredeyse garanti eder) işlemlere devam eder.
            {
                // --- 1. Resim Klasörünü Temizleme ---
                DirectoryInfo directory = new DirectoryInfo(picturePath); // Belirtilen resim yolu için bir dizin bilgisi nesnesi oluşturur.

                foreach (FileInfo file in directory.GetFiles()) // Dizin içindeki tüm dosyalar üzerinde döngü kurar.
                {
                    file.Delete(); // Bulunan her dosyayı (eski veya mevcut resimleri) siler.
                }

                // --- 2. Zip Dosyasını Çıkarma ---
                // Resimlerin toplu bulunduğu zip dosyasının tam yolunu oluşturur (setup dosyaları içindeki zip).
                string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");

                // Zip dosyasının içeriğini picturePath ile belirtilen dizine açar (çıkarır).
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }

    }
}
