using Keras.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Transforms;
using Numpy;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLTest
{
    public class DesignGenerator
    {
        private static MLContext mlContext;
        private static TensorFlowModel _tfModel;
        public static TensorFlowModel TFModel
        {
            get
            {
                if(_tfModel == null)
                {
                    mlContext = new MLContext();
                    _tfModel = mlContext.Model.LoadTensorFlowModel("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/frozenBoxModel.pb");
                    //DataViewSchema schema = TFModel.GetModelSchema();
                    //var featuresType = (VectorDataViewType)schema["layoutInput"].Type;
                    //var predictionType = (VectorDataViewType)schema["Identity"].Type;
                }
                return _tfModel;
            }
        }

        public List<Design> Mutated = new List<Design>();
        public List<Design> Targets = new List<Design>();
        public List<Design> Predictions = new List<Design>();

        Random rnd = new Random();


        public DesignGenerator(bool train, bool useModelData)
        {
            if (train)
            {
                GenerateTrainingData();
            }

            if (useModelData)
            {
                LoadTFData();
            }
            else
            {
                GenerateLocalData(50);
            }
            TestModel();
        }

        public void LoadTFData()
        {
             LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "testInputs.txt", Mutated);
             LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "testTargets.txt", Targets);
             LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "testPredictions.txt", Predictions);
        }

        public void LoadData(string folder, string inputFile, List<Design> container)
        {
            container.Clear();
            float val;
            IEnumerable<float> vals;
            StreamReader reader = new StreamReader(folder + inputFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                vals = values.Select(str => float.TryParse(str, out val) ? val : 0);
                container.Add(new Design(3, vals.ToArray()));
            }
            reader.Close();
        }

        public void GenerateTrainingData()
        {
            GenerateDataAt(100000, "D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "bx3_input.txt", "bx3_target.txt");
        }

        public void GenerateLocalData(int count)
        {
            Targets.Clear();
            Mutated.Clear();
            Predictions.Clear();
            for (int i = 0; i < count; i++)
            {
                Targets.Add(GenLayout());
            }
            TransformAll(Targets, Mutated);
            if (runModelOnNewData)
            {
                TestModel();
            }

            baseColor = new HSL((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }

        bool runModelOnNewData = false;
        public void TestModel(List<Design> mutatedInput = null, List<Design> outputPredictions = null)
        {
            mutatedInput = mutatedInput ?? Mutated;
            outputPredictions = outputPredictions ?? Predictions;

            runModelOnNewData = true;

            var pipeline = TFModel.ScoreTensorFlowModel( new[] { nameof(LayoutOutput.Identity) },new[] { nameof(LayoutInput.layoutInput) }, false);
            var dataView = mlContext.Data.LoadFromEnumerable(LayoutInput.GetInputs(Mutated));
            var estimator = pipeline.Fit(dataView);
            var transformedValues = estimator.Transform(dataView);

            var outScores = mlContext.Data.CreateEnumerable<LayoutOutput>(transformedValues, reuseRowObject: false);
            outputPredictions.Clear(); 
            foreach (var prediction in outScores)
            {
                //Console.WriteLine(string.Join(",", prediction.Identity));
                outputPredictions.Add(new Design(3, prediction.Identity));
            }
            //tensorFlowModel.Dispose();
        }

        Gaussian vGaussian = new Gaussian(0.5, 0.007);
        Gaussian nudgeGaussian = new Gaussian(0, 0.005);
        HSL baseColor = new HSL(0.9f, 0.7f, 0.3f);

        public Design GenLayout() => Design.GenLayout3((float)vGaussian.Sample(), (float)vGaussian.Sample());

        public void GenerateDataAt(int count, string folder, string inputFile, string targetFile)
        {
            var enc = new UTF8Encoding();
            StreamWriter inputStream = new StreamWriter(folder + inputFile, false, enc);
            StreamWriter targetStream = new StreamWriter(folder + targetFile, false, enc);

            inputStream.WriteLine("Var,ColH,ColS,ColL, Cx0,Cy0,Rx0,Ry0,Co0, Cx1,Cy1,Rx1,Ry1,Co1, Cx2,Cy2,Rx2,Ry2,Co2");
            targetStream.WriteLine("Cx0,Cy0,Rx0,Ry0,H0,S0,L0, Cx1,Cy1,Rx1,Ry1,H1,S1,L1, Cx2,Cy2,Rx2,Ry2,H2,S2,L2");
            for (int i = 0; i < count; i++)
            {
                var bx = GenLayout();
                var bxt = Transform(bx);
                bxt.InputSerialize(inputStream);
                bx.TargetSerialize(targetStream);
            }
            inputStream.Flush();
            targetStream.Flush();
            inputStream.Close();
            targetStream.Close();
        }

        public void RecolorDesigns(List<Design> designs, float stdDev = 0.03f)
        {
            foreach (var design in designs)
            {
                Design.ColorBoxesWithOffset(design, stdDev);
            }
        }

        public void TransformAll(List<Design> input, List<Design> container)
        {
            container.Clear();
            for (int i = 0; i < input.Count; i++)
            {
                container.Add(Transform(input[i]));
            }
            RecolorDesigns(container);
        }

        TruncatedGaussian colTransformOffset = new TruncatedGaussian(0, 0.06, -0.2, 0.2);
        //Gaussian colTransformOffset = new Gaussian(0, 0.01);
        private Design Transform(Design bx)
        {
            var result = bx.Clone();

            for (int i = 0; i < result.Count; i++)
            {
                result[i].Cx += (float)nudgeGaussian.Sample();
                result[i].Cy += (float)nudgeGaussian.Sample();
                result[i].Rx += (float)nudgeGaussian.Sample();
                result[i].Ry += (float)nudgeGaussian.Sample();
                // no need to transform color because it is already aproximated using variation and color offset in the input format
                //result[i].ColorOffset += (float)colTransformOffset.Sample();
            }

            return result;
        }

    }

    public class LayoutInput
    {
        [ColumnName("layoutInput")]
        [VectorType(19)]
        public float[] layoutInput;

        LayoutInput(float[] values) { layoutInput = values; }

        public static LayoutInput[] GetInputs(List<Design> layouts)
        {
            var result = new LayoutInput[layouts.Count];
            for (int i = 0; i < layouts.Count; i++)
            {
                result[i] = new LayoutInput(layouts[i].InputArray());
            }
            return result;
        }
    }

    public class LayoutOutput
    {
        [ColumnName("Identity")]
        [VectorType(21)]
        public float[] Identity;
    }

    public static class ShuffleExtension
    {
        private static Random rnd = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
