using System;
using System.Collections;
using System.Collections.Generic;

namespace recommender
{
	public class BooleanMatrix
	{
		public int NumOfRows 
		{
			get { return rowList.Count; }
		}
		public IList<ISet<int>> rowList = new List<ISet<int>>();

		public bool this [int x, int y]
		{
			get
			{
				if (x-1 < rowList.Count)
					return rowList[x-1].Contains(y);
				else
					return false;
			}
			set 
			{
				if (value == true)
				{
					if (this[x-1] != null)
					{
						this[x-1].Add(y);
					}
				}
				else
				{
					this[x-1].Remove(y);
				}
			}
		}

		public ICollection<int> this [int x]
		{
			get 
			{
				if (x >= rowList.Count)
					for (int i=rowList.Count; i<=x; ++i)
						rowList.Add(new HashSet<int>());
				return rowList[x];
			}
		}

		public BooleanMatrix ()
		{
		}
	}
}

