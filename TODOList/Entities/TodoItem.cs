namespace TODOList.Entities
    {
    public class TodoItem
        {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDone { get; set; }

        public virtual User User { get; set; }
        }
    }
