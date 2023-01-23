namespace Infrastructure.Interfaces
{
    public interface IBaseService<T> where T: class
    {
        public T GetById(string id);
        public List<T> GetAll();
    }
}
