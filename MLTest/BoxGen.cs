using Keras.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Probabilistic.Distributions;
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
    public class BoxGen
    {
        Random rnd = new Random();
        public List<Design> Mutated = new List<Design>();
        List<Design> targets = new List<Design>();
        List<Design> predictions = new List<Design>();

        public DrawTarget DrawTarget = DrawTarget.Truth;

        public BoxGen()
        {
            //GenerateTrainingData();
            LoadTFData();
            //GenerateLocalData();
            TestModel(Mutated);
        }

        public void LoadTFData()
        {
            Mutated = LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "testInputs.txt");
            targets = LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "testTargets.txt");
            predictions = LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "testPredictions.txt");
        }
        public List<Design> LoadData(string folder, string inputFile)
        {
            var result = new List<Design>();
            float val;
            IEnumerable<float> vals;
            StreamReader reader = new StreamReader(folder + inputFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                vals = values.Select(str => float.TryParse(str, out val) ? val : 0);
                result.Add(new Design(3, vals.ToArray()));
            }
            reader.Close();
            return result;
        }
        public void GenerateTrainingData()
        {
            GenerateDataAt(100000, "D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/", "bx3_input.txt", "bx3_target.txt");
        }
        public void GenerateLocalData()
        {
            targets.Clear();
            Mutated.Clear();
            predictions.Clear();
            for (int i = 0; i < 50; i++)
            {
                targets.Add(GenLayout());
            }
            Mutated = TransformAll(targets);
            if (runModelOnNewData)
            {
                TestModel(Mutated);
            }

            baseColor = new HSL((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }
        bool runModelOnNewData = false;
        public void TestModel(List<Design> mutatedInput)
        {
            runModelOnNewData = true;
            MLContext mlContext = new MLContext();
            var tensorFlowModel = mlContext.Model.LoadTensorFlowModel("D:/tmp/Python/PythonApplication1/PythonApplication1/boxModel/frozenBoxModel.pb");

            DataViewSchema schema = tensorFlowModel.GetModelSchema(); // Vector<Single, 12>
            Console.WriteLine(" =============== TensorFlow Model Schema =============== ");
            var featuresType = (VectorDataViewType)schema["layoutInput"].Type;
            Console.WriteLine($"Name: layoutInput, Type: {featuresType.ItemType.RawType}, Size: ({featuresType.Dimensions[0]})");
            var predictionType = (VectorDataViewType)schema["Identity"].Type;
            Console.WriteLine($"Name: Identity, Type: {predictionType.ItemType.RawType}, Size: ({predictionType.Dimensions[0]})");

            var dataView = mlContext.Data.LoadFromEnumerable(LayoutInput.GetInputs(Mutated));
            var pipeline = tensorFlowModel.ScoreTensorFlowModel( new[] { nameof(LayoutOutput.Identity) },new[] { nameof(LayoutInput.layoutInput) }, false);
            var estimator = pipeline.Fit(dataView);
            var transformedValues = estimator.Transform(dataView);

            var outScores = mlContext.Data.CreateEnumerable<LayoutOutput>(transformedValues, reuseRowObject: false);
            predictions = new List<Design>(); 
            foreach (var prediction in outScores)
            {
                Console.WriteLine(string.Join(",", prediction.Identity));
                predictions.Add(new Design(3, prediction.Identity));
            }

            tensorFlowModel.Dispose();
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
        public List<Design> TransformAll(List<Design> input)
        {
            var result = new List<Design>();
            for (int i = 0; i < input.Count; i++)
            {
                result.Add(Transform(input[i]));
            }
            RecolorDesigns(result);
            return result;
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

        public void OnDraw(Graphics g)
        {
            var toDraw = DrawTarget == DrawTarget.Truth ? targets : DrawTarget == DrawTarget.Mutated ? Mutated : predictions;
            for (int i = 0; i < toDraw.Count; i++)
            {
                var state = g.Save();
                ScaleTranslateTo(i, g);
                toDraw[i].Draw(g);
                g.Restore(state);
            }

            ScaleTranslateTo(toDraw.Count, g);
        }

        private void ScaleTranslateTo(int index, Graphics g)
        {
            int orgX = 50;
            int orgY = 30;
            int cols = 10;
            float w = 50;
            float h = 50;
            float marg = 20;
            float left = (index % cols) * (w + marg) + orgX;
            float top = (int)(index / cols) * (h + marg) + orgY;
            g.ScaleTransform(w, h);
            g.TranslateTransform(left / w, top / h);
        }
    }


    public enum DrawTarget
    {
        Truth,
        Mutated,
        Predictions,
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
