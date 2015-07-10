using System;

namespace recommender
{
	public class KNN
	{
		protected int maxRating = 5;
		public int MaxRating {
			get { return maxRating; }
			set { maxRating = value; }
		}
		protected int minRating = 1;
		public int MinRating {
			get { return minRating; }
			set { minRating = value; }
		}

		protected int k = 80;
		public int K
		{
			get { return k; }
			set { k = value; }
		}

		protected AggregationType aggregation;
		public AggregationType Aggregation { set { aggregation = value; } }

		public CorrelationType correlation;
		public CorrelationType Correlation { set { correlation = value; } }

		protected BooleanMatrix booleanRatings;
		public CorrelationMatrix corrMatrix;

		public KNN ()
		{
			booleanRatings = null;
			corrMatrix = null;
		}
	}
}

