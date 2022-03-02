
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

namespace Solution {
    
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
        if(map.ContainsKey(key))
        {
            var node = map[key] as LinkedListNode<(K, V)>;
            data.Remove(node);
            node = new LinkedListNode<(K, V)>( (node.Value.Item1, node.Value.Item2) );
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
        if(exists) {
            node = map[key] as LinkedListNode<(K, V)>;
            node.Value = (key, value);
            return false;
        }
        
        if(map.Count >= Capacity)
        {
            var (k, v) = data.First();
            map.Remove(k);
            data.RemoveFirst();
        }
        // Assert that count < size here
        node = new LinkedListNode<(K, V)>( (key, value) );
        map.Add(key, node);
        data.AddLast(node);
        return true;
    }
    
    public int Count()
    {
        return map.Count;
    }
}

class Solution {
    static void Main(string[] args) {
        var cache = new LRUCache<string, string>(2);
        
        if(!cache.Set("user-larry", "Larry"))
            Console.WriteLine("Add 'user-larry': Expected to add new key 'user-larry'");
        if(!cache.Set("user-curly", "Curly"))
            Console.WriteLine("Add 'user-curly': Expected to add new key 'user-curly'");
        
        var (u, e) = cache.Get("user-curly");
        if(!e || u != "Curly")
            Console.WriteLine($"Get 'user-curly': Expected 'Curly': {e}, {u}");
        (u, e) = cache.Get("user-larry");
        if(!e || u != "Larry")
            Console.WriteLine($"Get 'user-larry': Expected 'Larry': {e}, {u}");
        
        if(!cache.Set("user-moe", "Moe"))
            Console.WriteLine("Add 'user-moe': Expected to add new key 'user-moe'");

        (u, e) = cache.Get("user-curly");
        if(e || u != null)
            Console.WriteLine($"Get 'user-curly': Expected 'null': {e}, {u}");
        (u, e) = cache.Get("user-larry");
        if(!e || u != "Larry")
            Console.WriteLine($"Get 'user-larry': Expected 'Larry': {e}, {u}");
        (u, e) = cache.Get("user-moe");
        if(!e || u != "Moe")
            Console.WriteLine($"Get 'user-moe': Expected 'Moe': {e}, {u}");

        if(cache.Set("user-larry", "Lar-y"))
            Console.WriteLine("Update 'user-larry': Expected to update existing key");
            
        (u, e) = cache.Get("user-larry");
        if(!e || u != "Lar-y")
            Console.WriteLine($"Update 'user-larry': Expected 'Lar-y': {e}, {u}");
    }
}
}
