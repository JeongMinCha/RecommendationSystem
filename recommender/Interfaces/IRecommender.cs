using System;

namespace recommender
{
	public interface IRecommender
	{
		void Training (RatingData trainingata);
		float Predict (RatingData ratings, int user, int item);
		void WriteResult (string path, RatingData testData);
	}
}

