using System;

namespace com.ootii.Collections
{
	public sealed class ObjectPool<T> where T : new()
	{
		private int mGrowSize = 20;

		private T[] mPool;

		private int mNextIndex;

		public int Length => mPool.Length;

		public int Available => mPool.Length - mNextIndex;

		public int Allocated => mNextIndex;

		public ObjectPool(int rSize)
		{
			Resize(rSize, rCopyExisting: false);
		}

		public ObjectPool(int rSize, int rGrowSize)
		{
			mGrowSize = rGrowSize;
			Resize(rSize, rCopyExisting: false);
		}

		public T Allocate()
		{
			T result = default(T);
			if (mNextIndex >= mPool.Length)
			{
				if (mGrowSize <= 0)
				{
					return result;
				}
				Resize(mPool.Length + mGrowSize, rCopyExisting: true);
			}
			if (mNextIndex >= 0 && mNextIndex < mPool.Length)
			{
				result = mPool[mNextIndex];
				mNextIndex++;
			}
			return result;
		}

		public void Release(T rInstance)
		{
			if (mNextIndex > 0)
			{
				mNextIndex--;
				mPool[mNextIndex] = rInstance;
			}
		}

		public void Reset()
		{
			int rSize = mGrowSize;
			if (mPool != null)
			{
				rSize = mPool.Length;
			}
			Resize(rSize, rCopyExisting: false);
			mNextIndex = 0;
		}

		public void Resize(int rSize, bool rCopyExisting)
		{
			lock (this)
			{
				int num = 0;
				T[] array = new T[rSize];
				if (mPool != null && rCopyExisting)
				{
					num = mPool.Length;
					Array.Copy(mPool, array, Math.Min(num, rSize));
				}
				for (int i = num; i < rSize; i++)
				{
					array[i] = new T();
				}
				mPool = array;
			}
		}
	}
}
