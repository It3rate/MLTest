using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLTest
{
    public class Regress
    {
        Form _form;
        public Regress(Form f)
        {
            _form = f;
            Data1Regress();
        }
        void Data1Regress()
        {
            var mlContext = new MLContext();
            // Step one: load the data as an IDataView.
            // First, we define the loader: specify the data columns and where to find them in the text file.
            // Load the data into a data view. Remember though, loaders are lazy, so the actual loading will
            // happen when the data is accessed.
            var dataPath = "data1.txt";
            var trainData = mlContext.Data.LoadFromTextFile<RegressionData>(dataPath,
                // Default separator is tab, but the dataset has comma.
                separatorChar: ';'
            );

            // Sometime, caching data in-memory after its first access can save some loading time when the data
            // is going to be used several times somewhere. The caching mechanism is also lazy; it only caches
            // things after being used. User can replace all the subsequently uses of "trainData" with "cachedTrainData".
            // We still use "trainData" because a caching step, which provides the same caching function, will
            // be inserted in the considered "pipeline."
            var cachedTrainData = mlContext.Data.Cache(trainData);

            // Step two: define the learning pipeline. 

            // We 'start' the pipeline with the output of the loader.
            var pipeline =
                // First 'normalize' the data (rescale to be
                // between -1 and 1 for all examples)
                mlContext.Transforms.NormalizeMinMax("Features")
                // We add a step for caching data in memory so that the downstream iterative training
                // algorithm can efficiently scan through the data multiple times. Otherwise, the following
                // trainer will load data from disk multiple times. The caching mechanism uses an on-demand strategy.
                // The data accessed in any downstream step will be cached since its first use. In general, you only
                // need to add a caching step before trainable step, because caching is not helpful if the data is
                // only scanned once. This step can be removed if user doesn't have enough memory to store the whole
                // data set. Notice that in the upstream Transforms.Normalize step, we only scan through the data 
                // once so adding a caching step before it is not helpful.
                .AppendCacheCheckpoint(mlContext)
                // Add the SDCA regression trainer.
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Target", featureColumnName: "Features"));

            // Step three. Fit the pipeline to the training data.
            var model = pipeline.Fit(trainData);
            Console.WriteLine(model);
        }
        private class RegressionData
        {
            [LoadColumn(0, 10), ColumnName("Features")]
            public float FeatureVector { get; set; }

            [LoadColumn(11)]
            public float Target { get; set; }
        }
    }
}
