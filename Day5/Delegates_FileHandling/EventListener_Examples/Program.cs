using EventListener_Examples.Example;

class EventExample
{
    public static void Main(string[] args)
    {
        //Run();
        ////RunFunc();
        //File_Example e = new File_Example();
        //e.main();
        Json_Example j = new Json_Example();
        j.access();
    }

    /*
     * 
    public delegate void delegateName<T>(string msg);

    public static void Display<Type>(Type msg)
    {
        Console.WriteLine(msg);
    }
    public delegate void delegateDept<T>(string msg);

    public static void DisplayDept<Type>(Type msg)
    {
        Console.WriteLine(msg);
    }

    public static void Run()
    {
        delegateName<string> del = Display;
        del("Hello, World!");
        Action<string> action = Display;
        action("Hello, Action!");
        delegateDept<string> delDept = DisplayDept;
        delDept("Hello, Department!");
    }

    */

    //In-build delegate type using predicate
    //public static bool Display<T>(T name)
    //{
    //    Console.WriteLine(name);
    //    return true;
    //}
    //public static bool Add(int number)
    //{
    //   if(number > 0)
    //    {
    //        Console.WriteLine($"Number is positive: {number}");
    //        return true;
    //    }
    //    else
    //    {
    //        Console.WriteLine($"Number is not positive: {number}");
    //        return false;
    //    }
    //}
    //public static void Run()
    //{
    //    Predicate<string> predicate = Display;
    //    predicate("Hello, Predicate!");
    //    Predicate<int> predicateNum = Add;
    //    predicateNum(5);

    //}

    //Function delegate type using Func
    //public static int DisplayFunc<T>(T name)
    //{
    //    Console.WriteLine(name);
    //    return 1;
    //}

    //public static void RunFunc()
    //{
    //    Func<string, int> func = DisplayFunc;
    //    int result = func("Hello");
    //    Console.WriteLine($"Result: {result}");
    //}

    public static void CheckString(string s, Predicate<string> condition)
    {
        Console.WriteLine(condition(s));
    }
    public static bool IsPalindrome(string? s)
    {
        return s == new string(s.Reverse().ToArray());
    }


    public static void Run()
    {
        CheckString("malayalam", IsPalindrome);
        CheckString("Hello", (s) => s.Distinct().Count() == s.Length);
        CheckString("programming", s => s.Length > 8);
        CheckString("level", s => s[0] == s[s.Length - 1]);

    }



}








