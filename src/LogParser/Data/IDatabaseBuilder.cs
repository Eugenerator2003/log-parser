namespace LogParser.Data
{
    public interface IDatabaseBuilder<T> where T : IDatabaseSchema
    {
        public void CreateDatabase(T schema);
        public void RevisionDatabase(T schema);
    }
}
