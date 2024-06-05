namespace API_PCC.ApplicationModels
{
    public class AnimalPedigreeTree<T> where T: AnimalPedigreeModel
    {
        public Node<T> Root { get; private set; } = null!;

        public void Add(T value)
        {
            if (Root == null)
            {
                Root = new Node<T>(value);
            }
        }
        public void Add(Node<T> value)
        {
            Root = value;
        }

        public void AddSire(T value)
        {
            Root.AddSire(value);
        }

        public void AddDam(T value)
        {
            Root.AddDam(value);
        }

        public void AddSire(Node<T> value)
        {
            Root.AddSire(value);
        }

        public void AddDam(Node<T> value)
        {
            Root.AddDam(value);
        }


    }

    public class Node<T> where T : AnimalPedigreeModel
    {
        public T Value { get; private set; }
        public Node<T> Sire { get; private set; } = null!;
        public Node<T> Dam { get; private set; } = null!;
        
        public int level { get; set; }
        public Node(T value) => Value = value;

        public void AddSire(T newValue)
        {
            Sire = new Node<T>(newValue);
        }

        public void AddDam(T newValue)
        {
            Dam = new Node<T>(newValue);
        }

        public void AddSire(Node<T> newValue)
        {
            Sire = newValue;
        }

        public void AddDam(Node<T> newValue)
        {
            Dam = newValue;
        }
    }
}
