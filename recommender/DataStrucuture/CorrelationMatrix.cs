using System;
using System.Collections;
using System.Collections.Generic;

namespace recommender
{
	public class CorrelationMatrix
	{		
		private CorrelationType correlation = 0;
		public CorrelationType Correlation
		{ set { correlation = value; } }
		private int numEntities = 0;

		private readonly List<double[]> correlations;

		public double this [int id1, int id2]
		{
			get 
			{
				if (id1 == id2)
					return 0;

				int i1 = (id1 > id2) ? id1 : id2;
				int i2 = (id1 > id2) ? id2 : id1;

				return correlations[i1-1][i2-1];
			}
		}

		public List<int> CorrelatedEntities (int entityId)
		{
			var result = new List<int>();
			for (int i=0; i<numEntities; ++i)
			{
				if (this[i+1, entityId] > 0)
					result.Add(i+1);
			}
			// sort by similarity
			result.Sort(
				delegate(int i, int j) 
				{ 
					return this[j, entityId].CompareTo(this[i, entityId]); 
				});
			return result;
		}

		public CorrelationMatrix(int numEntities)
		{
			this.numEntities = numEntities;		
			correlations = new List<double[]>();
			for (int i=0; i<numEntities; ++i)
			{
				var doubles = new double[i];
				correlations.Add(doubles);
			}
			correlations.TrimExcess();
		}

		public void Construct (RatingData trainingData, int type)
		{
			if (type == Globals.userSim)
			{
				switch (correlation)
				{
				case CorrelationType.Cosine:
					ComputeCosineUserSimilarity(trainingData);
					break;
				case CorrelationType.Pearson:
					ComputePearsonUserSimilarity(trainingData);
					break;
				case CorrelationType.Intersection:
					ComputeIntersectionUserSimilarity(trainingData);
					break;
				}
			}
			else if (type == Globals.itemSim)
			{
				switch (correlation)
				{
				case CorrelationType.Cosine:
					ComputeCosineItemSimilarity(trainingData);
					break;
				case CorrelationType.Pearson:
					ComputePearsonItemSimilarity(trainingData);
					break;
				case CorrelationType.Intersection:
					ComputeIntersectionItemSimilarity(trainingData);
					break;
				}
			}
			Console.WriteLine("Correlation Matrix is constructed.");
		}

		private void ComputeCosineUserSimilarity (RatingData rating)
		{
			for (int x=1; x<=numEntities; ++x)
			{
				for (int y=1; y<=x-1; ++y)
				{
					double similarity = 0;
					double product_sum = 0;
					double square_sum1 = 0;
					double square_sum2 = 0;

//					float averageX = rating.UserAverage(x);
//					float averageY = rating.UserAverage(y);

					var commonItems = rating.CommonItems(x, y);
					if (commonItems.Count == 0)
					{
						similarity = 0;
					}
					else
					{
						foreach (int item in commonItems)
						{
							product_sum += rating[x,item]*rating[y,item];
							square_sum1 += rating[x,item]*rating[x,item];
							square_sum2 += rating[y,item]*rating[y,item];
//							product_sum += (rating[x,item]-averageX)*(rating[y,item]-averageX);
						}
						similarity = product_sum;
						similarity /= Math.Sqrt(square_sum1*square_sum2);
					}

					correlations[x-1][y-1] = similarity;
				}
			}
		}

		private void ComputeCosineItemSimilarity (RatingData rating)
		{
			for (int i=1; i<=numEntities; ++i)
			{
				for (int j=1; j<=i-1; ++j)
				{
					double similarity = 0;
					double product_sum = 0;
					double square_sum1 = 0;
					double square_sum2 = 0;

					var commonUsers = rating.CommonUsers(i, j);
					if (commonUsers.Count == 0)
					{
						similarity = 0;
					}
					else
					{
						foreach (int user in commonUsers)
						{
							product_sum += (rating[user,i])*(rating[user,j]);
							square_sum1 += (rating[user,i])*(rating[user,i]);
							square_sum2 += (rating[user,j])*(rating[user,j]);
						}

						similarity = product_sum;
						similarity /= Math.Sqrt(square_sum1*square_sum2);
					}

					correlations[i-1][j-1] = similarity;
				}
			}
		}

		private void ComputePearsonUserSimilarity (RatingData rating)
		{
			for (int x=1; x<=numEntities; ++x)
			{
				for (int y=1; y<=x-1; ++y)
				{
					double similarity = 0;
					double product_sum = 0;
					double square_sum1 = 0;
					double square_sum2 = 0;

					float averageX = rating.UserAverage(x);
					float averageY = rating.UserAverage(y);

					var commonItems = rating.CommonItems(x, y);
					if (commonItems.Count == 0)
					{
						similarity = 0;
					}
					else
					{
						foreach (int item in commonItems)
						{
							product_sum += (rating[x,item]-averageX)*(rating[y,item]-averageY);
							square_sum1 += (rating[x,item]-averageX)*(rating[x,item]-averageX);
							square_sum2 += (rating[y,item]-averageY)*(rating[y,item]-averageY);
						}
						similarity = product_sum;
						similarity /= Math.Sqrt(square_sum1*square_sum2);
					}

					correlations[x-1][y-1] = similarity;
				}
			}
		}

		private void ComputePearsonItemSimilarity (RatingData rating)
		{
			for (int i=1; i<=numEntities; ++i)
			{
				for (int j=1; j<=i-1; ++j)
				{
					double similarity = 0;
					double product_sum = 0;
					double square_sum1 = 0;
					double square_sum2 = 0;

//					float averageI = rating.ItemAverage(i);
//					float averageJ = rating.ItemAverage(i);

					var commonUsers = rating.CommonUsers(i, j);
					if (commonUsers.Count == 0)
					{
						similarity = 0;
					}
					else
					{
						foreach (int user in commonUsers)
						{
							float userAverage = rating.UserAverage(user);
							product_sum += (rating[user,i]-userAverage)*(rating[user,j]-userAverage);
							square_sum1 += (rating[user,i]-userAverage)*(rating[user,i]-userAverage);
							square_sum2 += (rating[user,j]-userAverage)*(rating[user,j]-userAverage);
//							product_sum += (rating[user,i]-averageI)*(rating[user,j]-averageJ);
//							square_sum1 += (rating[user,i]-averageI)*(rating[user,i]-averageI);
//							square_sum2 += (rating[user,j]-averageJ)*(rating[user,j]-averageJ);
						}

						similarity = product_sum;
						similarity /= Math.Sqrt(square_sum1*square_sum2);
					}

					correlations[i-1][j-1] = similarity;
				}
			}
		}

		private void ComputeIntersectionUserSimilarity (RatingData rating)
		{
			for (int user1=1; user1<=numEntities; ++user1)
			{
				for (int user2=1; user2<=user1-1; ++user2)
				{
					correlations[user1-1][user2-1] 
						= rating.CommonItems(user1, user2).Count;
				}
			}
		}

		private void ComputeIntersectionItemSimilarity (RatingData rating)
		{
			for (int item1=1; item1<=numEntities; ++item1)
			{
				for (int item2=1; item2<=item1-1; ++item2)
				{
					correlations[item1-1][item2-1] 
						= rating.CommonUsers(item1, item2).Count;
				}
			}
		}
	}
}

