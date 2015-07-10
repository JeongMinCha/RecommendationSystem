using System;
using System.Collections;
using System.Collections.Generic;

namespace recommender
{
	public class RatingData
	{
		private int maxUser = 0;
		public int MaxUser { 
			get { return maxUser; } 
			set { maxUser = value; }
		}

		private int maxItem = 0;
		public int MaxItem {
			get { return maxItem; }
			set { maxItem = value; }
		}
			
		public int Count {
			get { return users.Count; }
		}

		private List<int> users = null;
		private List<int> items = null;
		private List<float> scores = null;

		public List<int> Users { get { return users; } }
		public List<int> Items { get { return items; } }
		public List<float> Scores { get { return scores; } }

		private float[] averages_item;
		private float[] averages_user;

		private BitArray[] booleanRatings;
		private HashSet<int>[] ratedItems;
		private Dictionary<int, float>[] ratings_item;
		private Dictionary<int, float>[] ratings_user;

		public float this [int user, int item]
		{
			get {
				return ratings_user[user-1][item];
			}
		}

		public float UserAverage (int user)
		{
			return averages_user[user-1];
		}

		public float ItemAverage (int item)
		{
			return averages_item[item-1];
		}

		public bool RatingExists (int user, int item)
		{
			return booleanRatings[user-1][item-1];
		}

		public List<int> CommonItems (int user1, int user2)
		{
			var list = new List<int>();
			var dict1 = ratings_user[user1-1];
			var dict2 = ratings_user[user2-1];

			foreach (int item in dict1.Keys)
			{
				if (dict2.ContainsKey(item))
					list.Add(item);
			}
			return list;
		}

		public List<int> CommonUsers (int item1, int item2)
		{
			var list = new List<int>();
			var dict1 = ratings_item[item1-1];
			var dict2 = ratings_item[item2-1];

			foreach (int user in dict1.Keys)
			{
				if (dict2.ContainsKey(user))
					list.Add(user);
			}
			return list;
		}

		public HashSet<int> RatedItems (int user)
		{
			return ratedItems[user-1];
		}

		public RatingData (string filePath)
		{
			Preprocessing(filePath);

			ratings_item = new Dictionary<int, float>[MaxItem];
			for (int item=0; item<MaxItem; ++item)
				ratings_item[item] = new Dictionary<int, float>();

			ratings_user = new Dictionary<int, float>[MaxUser];
			for (int user=0; user<MaxUser; ++user)
				ratings_user[user] = new Dictionary<int, float>();

			ratedItems = new HashSet<int>[MaxUser];
			for (int user=0; user<MaxUser; ++user)
				ratedItems[user] = new HashSet<int>();

			booleanRatings = new BitArray[MaxUser];
			for (int user=0; user<MaxUser; ++user)
				booleanRatings[user] = new BitArray(MaxItem);
			
			averages_user = new float[MaxUser];
			averages_item = new float[MaxItem];

			Construct ();
		}

		private void Preprocessing (string filePath)
		{
			users = new List<int>();
			items = new List<int>();
			scores = new List<float>();

			int max_user = 0;
			int max_item = 0;
			string[] lines = System.IO.File.ReadAllLines(filePath);	
			foreach (string line in lines)
			{
				string[] words = line.Split('\t');
				if (words.Length >= 3)
				{
					int user = Convert.ToInt32(words[0]);
					int item = Convert.ToInt32(words[1]);
					float score = Convert.ToSingle(words[2]);

					users.Add(user);
					items.Add(item);
					scores.Add(score);

					if (max_user < user) 
						max_user = user;	
					if (max_item < item)
						max_item = item;
				}
			}
			maxUser = max_user;
			maxItem = max_item;

			users.TrimExcess();
			items.TrimExcess();
			scores.TrimExcess();
		}

		private void Construct ()
		{
			for (int index = 0; index < Count; ++ index)
			{
				int user = users[index];
				int item = items[index];
				float score = scores[index];

				ratings_user[user-1].Add(item, score);
				ratings_item[item-1].Add(user, score);
				ratedItems[user-1].Add(item);
				booleanRatings[user-1].Set(item-1, true);
			}
			CalculateAverageScoresForUsers();
			CalculateAverageScoresForItems();
		}

		private void CalculateAverageScoresForUsers()
		{
			// calculate averages of scores of users
			for (int user=1; user <= maxUser; ++ user)
			{
				int count = 0;
				float sum = 0;

				var userRatings = ratings_user[user-1];
				foreach (int item in userRatings.Keys)
				{
					sum += userRatings[item];
					count ++;
				}

				if (count == 0)
					averages_user[user-1] = 0;
				else
					averages_user[user-1] = sum / count;
			}
		}

		private void CalculateAverageScoresForItems()
		{
			// calculate averages of scores of items
			for (int item=1; item <= maxItem; ++ item)
			{
				int count = 0;
				float sum = 0;

				var itemRating = ratings_item[item-1];
				foreach (int user in itemRating.Keys)
				{
					sum += itemRating[user];
					count ++;
				}

				if (count == 0)
					averages_item[item-1] = 0;
				else
					averages_item[item-1] = sum / count;
			}
		}
	}
}