using com.ootii.Collections;

namespace com.ootii.Messages
{
	public class Message : IMessage
	{
		protected string mType = "";

		protected object mSender;

		protected object mRecipient;

		protected float mDelay;

		protected int mID;

		protected object mData;

		protected bool mIsSent;

		protected bool mIsHandled;

		protected int mFrameIndex;

		private static ObjectPool<Message> sPool = new ObjectPool<Message>(40, 10);

		public string Type
		{
			get
			{
				return mType;
			}
			set
			{
				mType = value;
			}
		}

		public object Sender
		{
			get
			{
				return mSender;
			}
			set
			{
				mSender = value;
			}
		}

		public object Recipient
		{
			get
			{
				return mRecipient;
			}
			set
			{
				mRecipient = value;
			}
		}

		public float Delay
		{
			get
			{
				return mDelay;
			}
			set
			{
				mDelay = value;
			}
		}

		public int ID
		{
			get
			{
				return mID;
			}
			set
			{
				mID = value;
			}
		}

		public object Data
		{
			get
			{
				return mData;
			}
			set
			{
				mData = value;
			}
		}

		public bool IsSent
		{
			get
			{
				return mIsSent;
			}
			set
			{
				mIsSent = value;
			}
		}

		public bool IsHandled
		{
			get
			{
				return mIsHandled;
			}
			set
			{
				mIsHandled = value;
			}
		}

		public int FrameIndex
		{
			get
			{
				return mFrameIndex;
			}
			set
			{
				mFrameIndex = value;
			}
		}

		public virtual void Clear()
		{
			mType = "";
			mSender = null;
			mRecipient = null;
			mID = 0;
			mData = null;
			mIsSent = false;
			mIsHandled = false;
			mDelay = 0f;
		}

		public virtual void Release()
		{
			Clear();
			IsSent = true;
			IsHandled = true;
			if (this != null)
			{
				sPool.Release(this);
			}
		}

		public static Message Allocate()
		{
			Message message = sPool.Allocate();
			message.IsSent = false;
			message.IsHandled = false;
			if (message == null)
			{
				message = new Message();
			}
			return message;
		}

		public static void Release(Message rInstance)
		{
			if (rInstance != null)
			{
				rInstance.IsSent = true;
				rInstance.IsHandled = true;
				sPool.Release(rInstance);
			}
		}

		public static void Release(IMessage rInstance)
		{
			if (rInstance != null)
			{
				rInstance.Clear();
				rInstance.IsSent = true;
				rInstance.IsHandled = true;
				if (rInstance is Message)
				{
					sPool.Release((Message)rInstance);
				}
			}
		}
	}
}
