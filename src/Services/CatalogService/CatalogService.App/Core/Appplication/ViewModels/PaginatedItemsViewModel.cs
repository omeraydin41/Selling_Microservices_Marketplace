namespace CatalogService.App.Core.Appplication.ViewModels
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        public int PageIndex { get; private set; }//hangi sayfadayiz

        public int PageSize { get; private set; }//sayfa basina kac urun gelecek

        public long Count { get; private set; }//toplam kac urun var

        public IEnumerable<TEntity> Data { get; private set; }//sayfadaki urunler liste şeklinde çağırılacak 

        public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)//constructor
        {
            PageIndex = pageIndex;//hangi sayfadayiz
            PageSize = pageSize;//sayfa basina kac urun gelecek
            Count = count;//toplam kac urun var
            Data = data;//sayfadaki urunler
        }
        //değiklenleri set edilemez yapıyoruz sadece constructor ile set edilebilir
    }
}
