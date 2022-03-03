
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

namespace Solution
{

    public class LRUCache<K, V>
    {
        public int Capacity { get; private set; }
        LinkedList<(K, V)> data;
        Hashtable map;

        public LRUCache(int size)
        {
            Capacity = size;
            data = new LinkedList<(K, V)>();
            map = new Hashtable();
        }

        public (V, bool) Get(K key)
        {
            if (map.ContainsKey(key))
            {
                var node = map[key] as LinkedListNode<(K, V)>;
                data.Remove(node);
                node = new LinkedListNode<(K, V)>((node.Value.Item1, node.Value.Item2));
                map[key] = node;
                data.AddLast(node);
                return (node.Value.Item2, true);
            }
            return (default(V), false);
        }

        // Return value indicates whether the key was added or already existed
        public bool Set(K key, V value)
        {
            LinkedListNode<(K, V)> node;
            var (_, exists) = Get(key);
            if (exists)
            {
                node = map[key] as LinkedListNode<(K, V)>;
                node.Value = (key, value);
                return false;
            }

            if (map.Count >= Capacity)
            {
                var (k, v) = data.First();
                map.Remove(k);
                data.RemoveFirst();
            }
            // Assert that count < size here
            node = new LinkedListNode<(K, V)>((key, value));
            map.Add(key, node);
            data.AddLast(node);
            return true;
        }

        public int Count()
        {
            return map.Count;
        }
    }

    class Solution
    {
        static void Main(string[] args)
        {
            var cache = new LRUCache<string, string>(2);

            if (!cache.Set("user-larry", "Larry"))
                Console.WriteLine("Add 'user-larry': Expected to add new key 'user-larry'");
            if (!cache.Set("user-curly", "Curly"))
                Console.WriteLine("Add 'user-curly': Expected to add new key 'user-curly'");

            var (u, e) = cache.Get("user-curly");
            if (!e || u != "Curly")
                Console.WriteLine($"Get 'user-curly': Expected 'Curly': {e}, {u}");
            (u, e) = cache.Get("user-larry");
            if (!e || u != "Larry")
                Console.WriteLine($"Get 'user-larry': Expected 'Larry': {e}, {u}");

            if (!cache.Set("user-moe", "Moe"))
                Console.WriteLine("Add 'user-moe': Expected to add new key 'user-moe'");

            (u, e) = cache.Get("user-curly");
            if (e || u != null)
                Console.WriteLine($"Get 'user-curly': Expected 'null': {e}, {u}");
            (u, e) = cache.Get("user-larry");
            if (!e || u != "Larry")
                Console.WriteLine($"Get 'user-larry': Expected 'Larry': {e}, {u}");
            (u, e) = cache.Get("user-moe");
            if (!e || u != "Moe")
                Console.WriteLine($"Get 'user-moe': Expected 'Moe': {e}, {u}");

            if (cache.Set("user-larry", "Lar-y"))
                Console.WriteLine("Update 'user-larry': Expected to update existing key");

            (u, e) = cache.Get("user-larry");
            if (!e || u != "Lar-y")
                Console.WriteLine($"Update 'user-larry': Expected 'Lar-y': {e}, {u}");


            string[][] dependencies = new string[][]{
                // group 1: P0 depends on P1 and so on
                new string[] {"P0", "P1"},
                new string[] {"P3", "P2"},
                new string[] {"P1", "P3"},
                new string[] {"P4", "P1"},
                //new string[] {"P2", "P3"},       // adding this makes it a cyclic dependency and not installable; negative case for part 1
//                new string[] {"P5", "P7"},
//                new string[] {"P6", "P5"},
                new string[] {"P0", "P8"},
                new string[] {"P1", "P8"},
        };
            /*
                    // group 2: add this to group 1 for next level, this is an additional dependency order
                    {"P5", "P7"},
                    {"P6", "P5"},

                    // group 3: add after group 2, this creates a diversion from the order in group 1
                    {"P0", "P8"},
                    {"P1", "P8"},

            ORDER: P2, P3, P1, P0, P4                   // group 1
                P2, P3, P1, P4, P0, P7, P5, P6       // group 1 + group 2
                P2, P3, P8, P1, P0, P4               // group 1 + group 3


            System.out.println(getInstallationOrder(dependencies).toString()); */

            var depGraph = new DependencyGraph();
            foreach (var dep in dependencies)
            {
                depGraph.AddDependency(dep[0], dep[1]);
            }

            var installOrder = depGraph.InstallOrder();
            Console.Write($"InstallOrder {installOrder.ToString()}: ");
            foreach (var part in installOrder)
            {
                Console.Write($"{part} ");
            }
            Console.WriteLine();

        }
    }

    public class Deps
    {
        public readonly HashSet<string> parents = new HashSet<string>();
        public readonly HashSet<string> children = new HashSet<string>();
    }
    public class DependencyGraph
    {
        private Dictionary<string, Deps> adjacencyList;

        public DependencyGraph()
        {
            adjacencyList = new Dictionary<string, Deps>();
        }

        public void AddDependency(string child, string parent)
        {
            if (!adjacencyList.ContainsKey(child))
            {
                adjacencyList.Add(child, new Deps());
            }
            if (!adjacencyList.ContainsKey(parent))
            {
                adjacencyList.Add(parent, new Deps());
            }

            adjacencyList[child].parents.Add(parent);
            adjacencyList[parent].children.Add(child);
        }

        public string[] InstallOrder()
        {
            var result = new List<string>();
            var installOrder = new List<string>();

            foreach (var part in adjacencyList.Keys)
            {
                installOrder.Add(part);
            }

            installOrder.Sort((l, r) => adjacencyList[l].parents.Count() - adjacencyList[r].parents.Count());
            Console.Write($"Provisional InstallOrder {installOrder.ToString()}: ");
            foreach (var part in installOrder)
            {
                Console.Write($"{part} ({adjacencyList[part].parents.Count()}) ");
            }
            Console.WriteLine();

            while (installOrder.Count() > 0
                && adjacencyList[installOrder[0]].parents.Count() == 0)
            {
                var part = installOrder[0];

                foreach (var child in adjacencyList[part].children)
                {
                    adjacencyList[child].parents.Remove(part);
                }

                adjacencyList.Remove(part);
                installOrder.RemoveAt(0);
                result.Add(part);
                installOrder.Sort((l, r) => adjacencyList[l].parents.Count() - adjacencyList[r].parents.Count());
            }

            if (installOrder.Count() > 0)
            {
                return new string[] { };
            }

            return result.ToArray();
        }
    }

}