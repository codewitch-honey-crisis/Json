using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Bee
{
	public static class CollectionUtility
	{
		public static IEnumerable<T> Synchronize<T>(this IEnumerable<T> collection,ReaderWriterLockSlim @lock=null)
		{
			if (null == @lock)
				@lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			var l = collection as IList<T>;
			if (null != l)
				return new _SyncList<T>(l, @lock);
			var c = collection as ICollection<T>;
			if (null != c)
				return new _SyncCollection<T>(c, @lock);
			if(null!=collection)
				return new _SyncEnumerable<T>(collection, @lock);
			return null;
		}
		public static IDictionary<TKey, TValue> Synchronize<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, ReaderWriterLockSlim @lock=null)
		{
			if (null == @lock)
				@lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			if (null != dictionary)
				return new _SyncDictionary<TKey, TValue>(dictionary, @lock);
			return null;
		}
		public static void CopyTo<TKey, TValue>(ICollection<KeyValuePair<TKey, TValue>> source, KeyValuePair<TKey, TValue>[] array, int index)
		{
			var i = source.Count;
			if (i > array.Length)
				throw new ArgumentException("The array is not big enough to hold the dictionary entries.", nameof(array));
			if (0 > index || i > array.Length + index)
				throw new ArgumentOutOfRangeException(nameof(index));
			i = 0;
			foreach (var item in source)
			{
				array[i + index] = item;
				++i;
			}
		}
		public static ICollection<TKey> CreateKeys<TKey, TValue>(IDictionary<TKey, TValue> parent)
			=> new _KeysCollection<TKey, TValue>(parent);
		public static ICollection<TValue> CreateValues<TKey, TValue>(IDictionary<TKey, TValue> parent)
			=> new _ValuesCollection<TKey, TValue>(parent);

		public static void AddRange<T>(this ICollection<T> collection,IEnumerable<T> items)
		{
			foreach (var item in items)
				collection.Add(item);
		}
		#region _KeysCollection
		private sealed class _KeysCollection<TKey, TValue> : ICollection<TKey>
		{
			const string _readOnlyMsg = "The collection is read-only.";
			IDictionary<TKey, TValue> _outer;
			public _KeysCollection(IDictionary<TKey, TValue> outer)
			{
				_outer = outer;
			}
			public bool Contains(TKey key)
				=> _outer.ContainsKey(key);
			public bool IsReadOnly => true;
			public void Add(TKey key)
				=> throw new InvalidOperationException(_readOnlyMsg);
			public bool Remove(TKey key)
				=> throw new InvalidOperationException(_readOnlyMsg);
			public void Clear()
				=> throw new InvalidOperationException(_readOnlyMsg);
			public int Count => _outer.Count;
			public IEnumerator<TKey> GetEnumerator()
			{
				foreach (var item in _outer)
					yield return item.Key;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
				=> GetEnumerator();
			public void CopyTo(TKey[] array, int index)
			{
				var i = _outer.Count;
				if (i > array.Length)
					throw new ArgumentException("The array is not big enough to hold the dictionary keys.", nameof(array));
				if (0 > index || i > array.Length + index)
					throw new ArgumentOutOfRangeException(nameof(index));
				i = 0;
				foreach (var item in _outer)
				{
					array[i + index] = item.Key;
					++i;
				}
			}
		}
		#endregion
		#region _ValuesCollection
		private sealed class _ValuesCollection<TKey, TValue> : ICollection<TValue>
		{
			const string _readOnlyMsg = "The collection is read-only.";
			IDictionary<TKey, TValue> _outer;
			public _ValuesCollection(IDictionary<TKey, TValue> outer)
			{
				_outer = outer;
			}
			public bool Contains(TValue value)
			{
				foreach (var item in _outer)
					if (Equals(item.Value, value))
						return true;
				return false;
			}
			public bool IsReadOnly => true;
			public void Add(TValue key)
				=> throw new InvalidOperationException(_readOnlyMsg);
			public bool Remove(TValue key)
				=> throw new InvalidOperationException(_readOnlyMsg);
			public void Clear()
				=> throw new InvalidOperationException(_readOnlyMsg);
			public int Count => _outer.Count;
			public IEnumerator<TValue> GetEnumerator()
			{
				foreach (var item in _outer)
					yield return item.Value;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
				=> GetEnumerator();
			public void CopyTo(TValue[] array, int index)
			{
				var i = _outer.Count;
				if (i > array.Length)
					throw new ArgumentException("The array is not big enough to hold the dictionary values.", nameof(array));
				if (0 > index || i > array.Length + index)
					throw new ArgumentOutOfRangeException(nameof(index));
				i = 0;
				foreach (var item in _outer)
				{
					array[i + index] = item.Value;
					++i;
				}
			}
		}
		#endregion

		#region _SyncEnumerator
		private sealed class _SyncEnumerator<T> : IEnumerator<T>
		{
			ReaderWriterLockSlim _lock;
			IEnumerator<T> _inner;
			public _SyncEnumerator(IEnumerator<T> inner, ReaderWriterLockSlim @lock)
			{
				_inner = inner;
				_lock = @lock;
			}

			public T Current {
				get {
					_lock.EnterReadLock();
					try
					{
						return _inner.Current;
					}
					finally
					{
						_lock.ExitReadLock();
					}
				}
			}
			object IEnumerator.Current => Current;

			public void Dispose()
			{
				_lock.EnterReadLock();
				try
				{
					_inner.Dispose();
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}

			public bool MoveNext()
			{
				_lock.EnterReadLock();
				try
				{
					return _inner.MoveNext();
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}

			public void Reset()
			{
				_lock.EnterReadLock();
				try
				{
					_inner.Reset();
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}
		}
		#endregion
		#region _SyncEnumerable
		private class _SyncEnumerable<T> : IEnumerable<T>
		{
			protected internal IEnumerable<T> Inner { get; }
			protected ReaderWriterLockSlim Lock { get; }
			public IEnumerator<T> GetEnumerator()
				=> new _SyncEnumerator<T>(Inner.GetEnumerator(), Lock);
			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
			public _SyncEnumerable(IEnumerable<T> inner, ReaderWriterLockSlim @lock)
			{
				Inner = inner;
				Lock = @lock;
			}
		}
		#endregion
		#region _SyncCollection
		private class _SyncCollection<T> : _SyncEnumerable<T>, ICollection<T>
		{
			public _SyncCollection(ICollection<T> inner, ReaderWriterLockSlim @lock) : base(inner, @lock) { }
			internal new ICollection<T> Inner { get { return base.Inner as ICollection<T>; } }


			public int Count {
				get {
					Lock.EnterReadLock();
					try
					{
						return Inner.Count;
					}
					finally
					{
						Lock.ExitReadLock();
					}
				}
			}
			public bool IsReadOnly {
				get {
					Lock.EnterReadLock();
					try
					{
						return Inner.IsReadOnly;
					}
					finally
					{
						Lock.ExitReadLock();
					}
				}
			}

			public void Add(T item)
			{
				Lock.EnterWriteLock();
				try
				{
					Inner.Add(item);
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}

			public void Clear()
			{
				Lock.EnterWriteLock();
				try
				{
					Inner.Clear();
				}
				catch
				{
					Lock.ExitWriteLock();
				}

			}

			public bool Contains(T item)
			{
				Lock.EnterReadLock();
				try
				{
					return Inner.Contains(item);
				}
				finally
				{
					Lock.ExitReadLock();
				}
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				Lock.EnterReadLock();
				try
				{
					Inner.CopyTo(array, arrayIndex);
				}
				finally
				{
					Lock.ExitReadLock();
				}
			}

			public bool Remove(T item)
			{
				Lock.EnterWriteLock();
				try
				{
					return Inner.Remove(item);
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}
		}
		#endregion
		#region _SyncList
		private class _SyncList<T> : _SyncCollection<T>, IList<T>
		{
			public _SyncList(IList<T> inner, ReaderWriterLockSlim @lock) : base(inner, @lock)
			{
			}
			public T this[int index] {
				get {
					Lock.EnterReadLock();
					try
					{
						return Inner[index];
					}
					finally
					{
						Lock.ExitReadLock();
					}

				}
				set {
					Lock.EnterWriteLock();
					try
					{
						Inner[index] = value;
					}
					finally
					{
						Lock.ExitWriteLock();
					}
				}
			}

			internal new IList<T> Inner { get { return base.Inner as IList<T>; } }


			public int IndexOf(T item)
			{
				Lock.EnterReadLock();
				try
				{
					return Inner.IndexOf(item);
				}
				finally
				{
					Lock.ExitReadLock();
				}
			}
			public void Insert(int index, T item)
			{
				Lock.EnterWriteLock();
				try
				{
					Inner.Insert(index, item);
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}

			public void RemoveAt(int index)
			{
				Lock.EnterWriteLock();
				try
				{
					Inner.RemoveAt(index);
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}
		}
		#endregion
		#region _SyncDictionary
		private class _SyncDictionary<TKey, TValue> : _SyncCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
		{
			public _SyncDictionary(IDictionary<TKey,TValue> inner, ReaderWriterLockSlim @lock) : base(inner,@lock) { }
			public TValue this[TKey key] {
				get {
					Lock.EnterReadLock();
					try
					{
						return Inner[key];
					}
					finally
					{
						Lock.ExitReadLock();
					}
				}
				set {
					Lock.EnterWriteLock();
					try
					{
						Inner[key] = value;
					}
					finally
					{
						Lock.ExitWriteLock();
					}
				}
			}

			public ICollection<TKey> Keys => new _SyncCollection<TKey>(Inner.Keys, Lock);
			public ICollection<TValue> Values => new _SyncCollection<TValue>(Inner.Values, Lock);
			internal new IDictionary<TKey,TValue> Inner {get {return base.Inner as IDictionary<TKey, TValue>; } }

			public void Add(TKey key, TValue value)
			{
				Lock.EnterWriteLock();
				try
				{
					Inner.Add(key, value);
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}

			public bool ContainsKey(TKey key)
			{
				Lock.EnterReadLock();
				try
				{
					return Inner.ContainsKey(key);
				}
				finally
				{
					Lock.ExitReadLock();
				}
			}

			public bool Remove(TKey key)
			{
				Lock.EnterWriteLock();
				try
				{
					return Inner.Remove(key);
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				Lock.EnterReadLock();
				try
				{
					return Inner.TryGetValue(key, out value);
				}
				finally
				{
					Lock.ExitReadLock();
				}
			}
		}
		#endregion
	}
}
