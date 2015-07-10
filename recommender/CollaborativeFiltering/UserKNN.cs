using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace recommender
{
	public class UserKNN : KNN, IRecommender
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

		public UserKNN() : base()
		{
			Console.WriteLine("UserKNN initialized.");
		}

		public void Training (RatingData trainingData)
		{
			corrMatrix = new CorrelationMatrix(ratings.MaxUser);
			corrMatrix.Correlation = correlation;
			corrMatrix.Construct(trainingData, Globals.userSim);
		}

		public float Predict (RatingData ratings, int user, int item)
		{
			int count = 0;			// # of similar users
			float prediction = 0;	// result of prediction
			float normFactor = 0;	// normalizing factor
			float sum = 0;			// linear sum, weighted sum, or weighted scaled sum
			int kThreshold = K;		// threshold k (Top-K similarities)

			var simUsers = corrMatrix.CorrelatedEntities(user);	// similar user group
			foreach (int simUser in simUsers)
			{
				if (booleanRatings[simUser, item])
				{
					float rating = ratings[simUser, item];
					float average = ratings.UserAverage(simUser);
					float weight = (float) corrMatrix[simUser, user];

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
				int integerScore = Convert.ToInt32(predictedScore);

				StringBuilder sb = new StringBuilder();
				sb.Append(user.ToString()).Append('\t').
				Append(item.ToString()).Append('\t').
				Append(integerScore.ToString());

				writer.WriteLine(sb.ToString());
			}
			writer.Close();
		}
	}
}