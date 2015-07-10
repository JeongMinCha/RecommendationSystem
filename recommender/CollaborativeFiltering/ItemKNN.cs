using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace recommender
{
	public class ItemKNN : KNN, IRecommender
	{
		private RatingData ratings;
		public RatingData Ratings
		{
			set 
			{
				ratings = value;
				booleanRatings = new BooleanMatrix();
				for (int index = 0; index < ratings.Count; ++index)
					booleanRatings[ratings.Users[index], ratings.Items[index]] = true;
			}
		}

		public ItemKNN() : base()
		{
		}

		public void Training (RatingData trainingData)
		{
			corrMatrix = new CorrelationMatrix(ratings.MaxItem);
			corrMatrix.Correlation = correlation;
			corrMatrix.Construct(trainingData, Globals.itemSim);
		}

		public float Predict (RatingData ratings, int user, int item)
		{
			int count = 0;			// # of similar items
			float prediction = 0;	// result of prediction
			float normFactor = 0;	// normalizing factor
			float sum = 0;			// linear sum, weighted sum, or weighted scaled sum
			uint kThreshold = 80;	// threshold k (Top-K similarities)

			var simItems = corrMatrix.CorrelatedEntities(item);	// similar item group
			foreach (int simItem in simItems)
			{
				if (booleanRatings[user, simItem])
				{
					float rating = ratings[user, simItem];
					float average = ratings.ItemAverage(simItem);
					float weight = (float) corrMatrix[simItem, item];

					count ++;
					normFactor += weight;
					switch (aggregation)
					{
					case AggregationType.NormalSum:
						sum += rating;
						break;
					case AggregationType.WeightedSum:
						sum += weight * rating;
						break;
					case AggregationType.WeightedScaledSum:
						sum += weight * (rating - average);
						break;
					}

					if (--kThreshold == 0)
						break;
				}
			}

			if (count == 0 || normFactor.Equals(0))
			{
				prediction = 0;
			}
			else
			{
				switch (aggregation)
				{
				case AggregationType.NormalSum:
					prediction = sum / count;
					break;
				case AggregationType.WeightedSum:
					prediction = sum / normFactor;
					break;
				case AggregationType.WeightedScaledSum:
					prediction = ratings.UserAverage(user);
					prediction += sum / normFactor;
					break;
				}
			}

			if (prediction > MaxRating)
				prediction = MaxRating;
			if (prediction < MinRating)
				prediction = MinRating;

			return prediction;
		}

		public void WriteResult (string resultFilePath, RatingData testData)
		{
			StreamWriter writer = new StreamWriter(resultFilePath);		
			for (int index = 0; index < testData.Count; ++ index)
			{
				int user = testData.Users[index];
				int item = testData.Items[index];
				float predictedScore = Predict(ratings, user, item);

				StringBuilder sb = new StringBuilder();
				sb.Append(user.ToString()).Append('\t').
				Append(item.ToString()).Append('\t').
				Append(predictedScore.ToString());

				writer.WriteLine(sb.ToString());
			}
			writer.Close();
		}
	}
}

