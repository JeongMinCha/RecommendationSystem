using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace recommender
{
	public static class Globals
	{
		public static int userSim = 1;
		public static int itemSim = 2;
	}

	class MainClass
	{
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			CheckArguments(args);		// check arguments

			/* file names of training file and test file. */
			string trainingFile = args[0];
			string testFile = args[1];
			string resultFile = args[0] + "_prediction.txt";


			/* instances of DataSet class representing training and test set. */
			var trainingData = new RatingData(trainingFile);
			var testData = new RatingData(testFile);

			Console.WriteLine("rating matrices for both files are created.");

			var recommender = new UserKNN();
			recommender.Ratings = trainingData;
			recommender.K = 60;
			recommender.Correlation = CorrelationType.Pearson;
			recommender.Aggregation = AggregationType.WeightedScaledSum;

			recommender.Training(trainingData);
			recommender.WriteResult(resultFile, testData);

			PrintRMSE(trainingFile);
		}

		private static void PrintRMSE (string inputFile)
		{
			if (File.Exists(inputFile) == false)
				return;

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = "PA4.exe";
			psi.Arguments = inputFile.Replace(".base", null);
			psi.RedirectStandardOutput = true;
			psi.UseShellExecute = false;

			Process proc = Process.Start(psi);
			proc.WaitForExit();

			string txt = proc.StandardOutput.ReadToEnd();
			Console.WriteLine(txt);
		}

		/// <summary>
		/// Checks the arguments.
		/// </summary>
		/// <param name="args">array of arguments.</param>
		private static void CheckArguments(string[] args)
		{
			if (args.Length != 2) {
				PrintUsage();
			} else if (File.Exists(args[0]) == false ||
				File.Exists(args[1]) == false) {
				PrintUsage();
			} 
		}

		/// <summary>
		/// Prints the usage of this program.
		/// </summary>
		private static void PrintUsage()
		{
			Console.WriteLine("recommender.exe [training file] [test file]");
			Console.WriteLine("You should input names of existing files.");
			Console.WriteLine("ex) recommender.exe u1.base u2.base");
			System.Environment.Exit(-1);
		}
	}
}