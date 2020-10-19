using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Probabilistic.Distributions;
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
    public class LayoutInput
    {
        [VectorType(12)]
        public float[] x;
        LayoutInput(float[] values) { x = values; }

        public static LayoutInput[] GetInputs(List<Layout> layouts)
        {
            var result = new LayoutInput[layouts.Count];
            for (int i = 0; i < layouts.Count; i++)
            {
                result[i] = new LayoutInput(layouts[i].AsFloatArray());
            }
            return result;
        }
    }
    
    public class LayoutOutput
    {
        [VectorType(12)]
        public float[] Identity;
    }
    public class BoxGen
    {
        Random rnd = new Random();
        List<Layout> mutated = new List<Layout>();
        List<Layout> targets = new List<Layout>();
        List<Layout> predictions = new List<Layout>();

        public DrawTarget DrawTarget = DrawTarget.Mutated;

        public BoxGen()
        {
            //GenerateTrainingData();
            GenerateLocalData();
            TestModel(mutated);
        }

        public void LoadData()
        {
            mutated = LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxData/", "testInputs.txt");
            targets = LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxData/", "testTargets.txt");
            predictions = LoadData("D:/tmp/Python/PythonApplication1/PythonApplication1/boxData/", "testPredictions.txt");
        }
        public void GenerateTrainingData()
        {
            GenerateDataAt(50000, "D:/tmp/Python/PythonApplication1/PythonApplication1/boxData/", "bx3_input.txt", "bx3_target.txt");
        }
        public void GenerateLocalData()
        {
            for (int i = 0; i < 50; i++)
            {
                targets.Add(GenBox());
            }
            mutated = TransformAll(targets);
        }
        public void TestModel(List<Layout> mutatedInput)
        {
            MLContext mlContext = new MLContext();
            var tensorFlowModel = mlContext.Model.LoadTensorFlowModel("D:/tmp/Python/PythonApplication1/PythonApplication1/boxData/frozenBoxModel.pb");

            DataViewSchema schema = tensorFlowModel.GetModelSchema(); // Vector<Single, 12>
            Console.WriteLine(" =============== TensorFlow Model Schema =============== ");
            var featuresType = (VectorDataViewType)schema["x"].Type;
            Console.WriteLine($"Name: x, Type: {featuresType.ItemType.RawType}, Size: ({featuresType.Dimensions[0]})");
            var predictionType = (VectorDataViewType)schema["Identity"].Type;
            Console.WriteLine($"Name: Identity, Type: {predictionType.ItemType.RawType}, Size: ({predictionType.Dimensions[0]})");


            var dataView = mlContext.Data.LoadFromEnumerable(LayoutInput.GetInputs(mutated));
            //var pipeline = tensorFlowModel.ScoreTensorFlowModel("Identity", "x", false);
            var pipeline = tensorFlowModel.ScoreTensorFlowModel( new[] { nameof(LayoutOutput.Identity) },new[] { nameof(LayoutInput.x) },false);
            var estimator = pipeline.Fit(dataView);
            var transformedValues = estimator.Transform(dataView);

            var outScores = mlContext.Data.CreateEnumerable<LayoutOutput>(transformedValues, reuseRowObject: false);

            predictions = new List<Layout>(); 
            foreach (var prediction in outScores)
            {
                predictions.Add(new Layout(prediction.Identity));
            }
            Console.WriteLine(outScores);
            //estimator.Fit()
            //var engine = mlContext.Model.CreatePredictionEngine<LayoutType, LayoutType> (tensorFlowModel);  //<Vector<Single, 12>, Vector<Single, 12>> (model);


            tensorFlowModel.Dispose();

            //IDataView dataView = mlContext.Data.LoadFromEnumerable(new List<MovieReview>());
            //ITransformer model = pipeline.Fit(dataView);
            //estimator.Fit(input);
            //ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true)

        }
        public void GenerateDataAt(int count, string folder, string inputFile, string targetFile)
        {
            var enc = new UTF8Encoding();
            StreamWriter inputStream = new StreamWriter(folder + inputFile, false, enc);
            StreamWriter targetStream = new StreamWriter(folder + targetFile, false, enc);

            inputStream.WriteLine("Cx0,Cy0,Rx0,Ry0,Cx1,Cy1,Rx1,Ry1,Cx2,Cy2,Rx2,Ry2");
            targetStream.WriteLine("Cx0,Cy0,Rx0,Ry0,Cx1,Cy1,Rx1,Ry1,Cx2,Cy2,Rx2,Ry2");
            for (int i = 0; i < count; i++)
            {
                var bx = GenBox();
                var bxt = Transform(bx);
                inputStream.WriteLine(bxt.ToString());
                targetStream.WriteLine(bx.ToString());
            }
            inputStream.Flush();
            targetStream.Flush();
            inputStream.Close();
            targetStream.Close();
        }
        public List<Layout> LoadData(string folder, string inputFile)
        {
            var result = new List<Layout>();
            double val;
            IEnumerable<double> vals;
            StreamReader reader = new StreamReader(folder + inputFile);
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                vals = values.Select(str => double.TryParse(str, out val) ? val : 0);
                result.Add(new Layout(vals.ToArray()));
            }
            reader.Close();
            return result;
        }

        Gaussian vGaussian = new Gaussian(0.5, 0.007);
        Gaussian dispGaussian = new Gaussian(0, 0.005);
        public Layout GenBox()
        {
            Layout result = new Layout(3);
            float v = (float)vGaussian.Sample();
            float h = (float)vGaussian.Sample();
            var boxes = new List<Box> { result[0], result[1], result[2] };

            bool wideBot = v > 0.5;
            boxes[0].Cx = 0.5f;
            boxes[0].Cy = wideBot ? v / 2.0f : 1f - (1f - v) / 2.0f;
            boxes[0].Rx = 0.5f;
            boxes[0].Ry = wideBot ? v / 2.0f : (1.0f - v) / 2.0f;

            boxes[1].Cx = h / 2.0f;
            boxes[1].Cy = wideBot ? v + (1.0f - v) / 2.0f : v / 2.0f;
            boxes[1].Rx = h / 2.0f;
            boxes[1].Ry = wideBot ? (1.0f - v) / 2.0f : v / 2.0f;

            boxes[2].Cx = h + (1.0f - h) / 2.0f;
            boxes[2].Cy = boxes[1].Cy;
            boxes[2].Rx = (1.0f - h) / 2.0f;
            boxes[2].Ry = boxes[1].Ry;

            if(rnd.NextDouble() > 0.5)
            {
                result.Rotate();
            }
            boxes.Shuffle();

            return result;
        }
        public List<Layout> TransformAll(List<Layout> input)
        {
            var result = new List<Layout>();
            for(int i = 0; i < input.Count; i++)
            {
                result.Add(Transform(input[i]));
            }
            return result;
        }
        private Layout Transform(Layout bx)
        {
            var result = bx.Clone();

            for (int i = 0; i < result.Count; i++)
            {
                result[i].Cx += (float)dispGaussian.Sample();
                result[i].Cy += (float)dispGaussian.Sample();
                result[i].Rx += (float)dispGaussian.Sample();
                result[i].Ry += (float)dispGaussian.Sample();
            }

            return result;
        }

        Layout layoutOfInterest = new Layout(0.17118305, 0.2429369,  0.16565311, 0.24538253, 0.66856414, 0.25028878, 0.31348467, 0.23988356, 0.48624027, 0.7277348,  0.50574875, 0.24975622);
        public void OnDraw(Graphics g)
        {
            var toDraw = DrawTarget == DrawTarget.Truth ? targets : DrawTarget == DrawTarget.Mutated ? mutated : predictions;
            for (int i = 0; i < toDraw.Count; i++)
            {
                var state = g.Save();
                ScaleTranslateTo(i, g);
                toDraw[i].Draw(g);
                g.Restore(state);
            }

            ScaleTranslateTo(toDraw.Count, g);
            layoutOfInterest.Draw(g);
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
