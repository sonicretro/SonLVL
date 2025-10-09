using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SonicRetro.SonLVL.API
{
	public class MultiFileIndexer<T> : IList<T>
	{
		private class MFIEntry : IComparable<MFIEntry>
		{
			public List<T> Data { get; set; }
			public int Offset { get; set; }
			public bool Fixed { get; private set; }
			public int Count => Data.Count;

			public MFIEntry(List<T> data, int offset, bool @fixed)
			{
				Data = data;
				Offset = offset;
				Fixed = @fixed;
			}

			public int CompareTo(MFIEntry other)
			{
				return Offset.CompareTo(other.Offset);
			}
		}

		private List<MFIEntry> files = new List<MFIEntry>();
		Func<T> defaultItem;

		public MultiFileIndexer()
		{
			defaultItem = () => default(T);
		}

		public MultiFileIndexer(Func<T> defaultItem)
		{
			this.defaultItem = defaultItem;
		}

		public void AddFile(List<T> data, int offset)
		{
			bool fix = offset != -1;
			files.Add(new MFIEntry(data, fix ? offset : Count, fix));
		}

		private int GetContainingFile(int index)
		{
			for (int i = files.Count - 1; i >= 0; i--)
			{
				if (index >= files[i].Offset && index - files[i].Offset < files[i].Count)
					return i;
			}
			return -1;
		}

		public void FillGaps()
		{
			int cnt = Count;
			for (int i = 1; i < cnt; i++)
				if (GetContainingFile(i) == -1)
					InsertAfter(i - 1, defaultItem());
		}

		public T this[int index]
		{
			get
			{
				int i = GetContainingFile(index);
				if (i == -1) return default(T);
				return files[i].Data[index - files[i].Offset];
			}
			set
			{
				int i = GetContainingFile(index);
				files[i].Data[index - files[i].Offset] = value;
			}
		}

		public void Add(T item)
		{
			files.Max().Data.Add(item);
		}

		public void InsertBefore(int index, T insertItem)
		{
			int i = GetContainingFile(index);
			files[i].Data.Insert(index - files[i].Offset, insertItem);
			for (i++; i < FileCount; i++)
				if (!files[i].Fixed)
					files[i].Offset++;
		}

		public void InsertAfter(int index, T insertItem)
		{
			int i = GetContainingFile(index);
			if (i + 1 < files.Count && files[i + 1].Offset == files[i].Offset + files[i].Count)
				++i;
			files[i].Data.Insert(index - files[i].Offset + 1, insertItem);
			for (i++; i < FileCount; i++)
				if (!files[i].Fixed)
					files[i].Offset++;
		}

		public bool Remove(T item)
		{
			int i = GetContainingFile(ToList().IndexOf(item));
			if (i == -1) return false;
			files[i].Data.Remove(item);
			for (i++; i < FileCount; i++)
				if (!files[i].Fixed)
					files[i].Offset--;
			FillGaps();
			return true;
		}

		public void RemoveAt(int index)
		{
			int i = GetContainingFile(index);
			files[i].Data.RemoveAt(index - files[i].Offset);
			for (i++; i < FileCount; i++)
				if (!files[i].Fixed)
					files[i].Offset--;
			FillGaps();
		}

		public bool ContainsIndex(int index) => GetContainingFile(index) != -1;

		public void Clear()
		{
			files.Clear();
		}

		public int Count
		{
			get
			{
				int maxind = 0;
				for (int i = 0; i < files.Count; i++)
					maxind = Math.Max(files[i].Offset + files[i].Count, maxind);
				return maxind;
			}
		}

		public int FileCount { get { return files.Count; } }

		public List<T> ToList()
		{
			List<T> res = new List<T>();
			foreach (T item in this)
			{
				res.Add(item);
			}
			return res;
		}

		public T[] ToArray()
		{
			return ToList().ToArray();
		}

		public ReadOnlyCollection<T> GetFile(int file)
		{
			return new ReadOnlyCollection<T>(files[file].Data);
		}
        
		public ReadOnlyCollection<ReadOnlyCollection<T>> GetFiles()
		{
			return files.Select(a => a.Data.AsReadOnly()).ToList().AsReadOnly();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new MultiFileEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new MultiFileEnumerator(this);
		}

		public class MultiFileEnumerator : IEnumerator<T>
		{
			private int current = -1;
			private MultiFileIndexer<T> indexer;

			public MultiFileEnumerator(MultiFileIndexer<T> indexer)
			{
				this.indexer = indexer;
			}

			T IEnumerator<T>.Current
			{
				get { return indexer[current]; }
			}

			void IDisposable.Dispose()
			{

			}

			object IEnumerator.Current
			{
				get { return indexer[current]; }
			}

			bool IEnumerator.MoveNext()
			{
				current++;
				return current < indexer.Count;
			}

			void IEnumerator.Reset()
			{
				current = -1;
			}
		}

		public int IndexOf(T item)
		{
			return ToList().IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			InsertBefore(index, item);
		}

		public bool Contains(T item)
		{
			return ToList().Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			ToList().CopyTo(array, arrayIndex);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}
	}
}
