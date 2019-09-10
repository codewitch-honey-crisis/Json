using Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTextReaderDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			// very long. Uncomment if you want it
			//_Example1();
			_Example2();
			_Example3();
			_Example4();
			_Example5();
			_Example6();
			_Example7();
			// bonus code (not in article)
			using (var reader = JsonTextReader.CreateFrom(@"..\..\data.json"))
			{
				if (reader.SkipTo("seasons", 7, "episodes", 3, "guest_stars", 0, "character"))
				{
					// move past the field name if it's a key.
					if (JsonNodeType.Key != reader.NodeType || reader.Read())
					{
						Console.WriteLine(reader.ParseSubtree());
					}
				}
			}
		}
		static void _Example1()
		{
			using (var reader = JsonTextReader.CreateFrom(@"..\..\data.json"))
			{
				while (reader.Read())
				{
					Console.Write(reader.NodeType);
					// only keys and values have a Value
					if (JsonNodeType.Value == reader.NodeType
						|| JsonNodeType.Key == reader.NodeType)
						Console.Write(" " + reader.Value);
					Console.WriteLine();
				}
			}
		}
		static void _Example2()
		{
			var url = "http://api.themoviedb.org/3/tv/2129?api_key=c83a68923b7fe1d18733e8776bba59bb";
			using (var reader = JsonTextReader.CreateFromUrl(url))
			{
				if (reader.SkipToField("created_by"))
				{
					// we're currently on the *key*/field name
					// we have to move to the value if we want 
					// just that.
					// finally, we parse the subtree into a
					// tree of objects, and write that to the
					// console, which pretty prints it
					if (reader.Read())
						Console.WriteLine(reader.ParseSubtree());
					else // below should never execute
						Console.WriteLine("Sanity check failed, key has no value");
				}
				else
					Console.WriteLine("Not found");
			}
		}
		static void _Example3()
		{
			using (var reader = JsonTextReader.CreateFromUrl("http://api.themoviedb.org/3/tv/2129?api_key=c83a68923b7fe1d18733e8776bba59bb"))
			{
				if (reader.SkipToField("created_by"))
				{
					// we're currently on the *key*/field name
					// we have to move to the value if we want 
					// just that.
					if (reader.Read())
					{
						// now, skip to the index we want
						// underneath where we are.
						// finally, we parse the subtree into a
						// tree of objects, and write that to the
						// console, which pretty prints it
						if (reader.SkipToIndex(1))
							Console.WriteLine(reader.ParseSubtree());
						else
							Console.WriteLine("Couldn't find the index.");
					}
					else // below should never execute
						Console.WriteLine("Sanity check failed, key has no value");
				}
				else
					Console.WriteLine("Not found");
			}
		}
		static void _Example4()
		{
			using (var reader = JsonTextReader.CreateFromUrl("http://api.themoviedb.org/3/tv/2129?api_key=c83a68923b7fe1d18733e8776bba59bb"))
			{
				// skip to "$.created_by[1]" <-- JSON path syntax
				if (reader.SkipTo("created_by", 1))
					Console.WriteLine(reader.ParseSubtree());
				else
					Console.WriteLine("Not found");
			}
		}
		static void _Example5()
		{
			var url = "http://api.themoviedb.org/3/tv/2129?api_key=c83a68923b7fe1d18733e8776bba59bb";
			using (var reader = JsonTextReader.CreateFromUrl(url))
			{
				// skip to "$.created_by[1].name" <-- JSON path syntax
				if (reader.SkipTo("created_by", 1, "name"))
				{
					// we're currently on the *key*/field name
					// we have to move to the value if we want 
					// just that.
					if (reader.Read())
						Console.WriteLine(reader.ParseSubtree());
					else // below should never execute
						Console.WriteLine("Sanity check failed, key has no value");
				}
				else
					Console.WriteLine("Not found");
			}
		}
		static void _Example6()
		{
			var url = "http://api.themoviedb.org/3/tv/2129?api_key=c83a68923b7fe1d18733e8776bba59bb";
			using (var reader = JsonTextReader.CreateFromUrl(url))
			{
				// accept either "seasons" or "production_companies" - whichever comes first
				if (reader.SkipToAnyOfFields("seasons", "production_companies"))
				{
					// we're currently on the *key*/field name
					// we have to move to the value if we want 
					// just that.
					if (reader.Read())
						Console.WriteLine(reader.ParseSubtree());
					else // below should never execute
						Console.WriteLine("Sanity check failed, key has no value");
				}
				else
					Console.WriteLine("Not found");
			}
		}
		static void _Example7()
		{
			var url = "http://api.themoviedb.org/3/tv/2129?api_key=c83a68923b7fe1d18733e8776bba59bb";
			using (var reader = JsonTextReader.CreateFromUrl(url))
			{
				// skip to "$.created_by[1].name" <-- JSON path syntax
				if (reader.SkipTo("created_by", 0, "name"))
				{
					// we're currently on the *key*/field name
					// we have to move to the value if we want 
					// just that.
					if (reader.Read())
						Console.WriteLine(reader.ParseSubtree());
					else // below should never execute
						Console.WriteLine("Sanity check failed, key has no value");
					// we need to move outward in the tree 
					// so we can read the next array element
					// so we skip the rest of this object
					reader.SkipToEndObject();
					if (reader.Read()) // read past the end of the object
					{
						// we're on the next object so get the name
						reader.SkipToField("name");
						// we're currently on the *key*/field name
						if (reader.Read())
							Console.WriteLine(reader.ParseSubtree());
						else // below should never execute
							Console.WriteLine("Sanity check failed, key has no value");
					}
					else // below should never execute, we didn't expect to reach the end
						Console.WriteLine("Sanity check failed, unexpected end of document");

				}
				else
					Console.WriteLine("Not found");
			}
		}
	}
}
