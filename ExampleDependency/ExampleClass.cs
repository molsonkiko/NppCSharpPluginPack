namespace ExampleDependency
{
    public class ExampleClass
    {
        public static int ExampleClassInstancesCreated = 0;
        public string Name = "";
        public int InstancesCreatedBeforeThis { get; private set; }

        public ExampleClass(string name)
        {
            Name = name;
            InstancesCreatedBeforeThis = ExampleClassInstancesCreated;
            ExampleClassInstancesCreated = Add(ExampleClassInstancesCreated, 1);
        }

        public override string ToString()
        {
            return $"ExampleClass(\"{Name}\") ({InstancesCreatedBeforeThis} instances created before it)";
        }

        public static int Add(int x, int y)
        {
            return x + y;
        }

        public static int Subtract(int x, int y)
        {
            return x - y;
        }
    }
}
