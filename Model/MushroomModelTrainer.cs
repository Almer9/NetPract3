using Microsoft.ML;
using MLTEST;
using System.Globalization;
using Tensorflow.Util;

namespace NetPract3.Model
{
    public class MushroomModelTrainer
    {
        static string _appPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) ?? ".";
        static string _trainDataPath = Path.Combine(_appPath, "..", "..", "..", "Resources", "mushroom.tsv");
        static string _modelPath = Path.Combine(_appPath, "..", "..", "..", "Models", "model.zip");

        static MLContext _mlContext;
        static PredictionEngine<Mushroom, MushroomPrediction> _predEngine;
        static ITransformer _trainedModel;
        static IDataView _trainingDataView;

        public MushroomModelTrainer()
        {
            LoadModel();
        }


        public string LoadModel()
        {
            _mlContext = new MLContext();
            _trainingDataView = _mlContext.Data.LoadFromTextFile<Mushroom>(_trainDataPath, hasHeader: true);

            if (File.Exists(_modelPath))
            {
                _trainedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);
                _predEngine = _mlContext.Model.CreatePredictionEngine<Mushroom, MushroomPrediction>(_trainedModel);
                return "Loaded model from file succesfully!";

            }
            else 
            {
                return TrainModel();
            }
        }
        public string TrainModel()
        {
            try
            {
                var pipeline = ProcessData();
                var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);
                SaveModelAsFile(_mlContext, _trainingDataView.Schema, _trainedModel);
                return "Model was trained succesfully!";
            }
            catch (Exception ex) 
            {
                return ex.Message;
            }
        }
        static IEstimator<ITransformer> ProcessData()
        {
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "edibility", outputColumnName: "Label")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "capShape", outputColumnName: "capShapeEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "capSurface", outputColumnName: "capSurfaceEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "capColor", outputColumnName: "capColorEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "bruises", outputColumnName: "bruisesEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "odor", outputColumnName: "odorEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "gillAttachment", outputColumnName: "gillAttachmentEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "gillSpacing", outputColumnName: "gillSpacingEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "gillSize", outputColumnName: "gillSizeEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "gillColor", outputColumnName: "gillColorEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "stalkShape", outputColumnName: "stalkShapeEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "stalkRoot", outputColumnName: "stalkRootEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "stalkSurfaceAboveRing", outputColumnName: "stalkSurfaceAboveRingEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "stalkSurfaceBelowRing", outputColumnName: "stalkSurfaceBelowRingEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "stalkColorAboveRing", outputColumnName: "stalkColorAboveRingEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "stalkColorBelowRing", outputColumnName: "stalkColorBelowRingEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "veilType", outputColumnName: "veilTypeEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "veilColor", outputColumnName: "veilColorEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "ringNumber", outputColumnName: "ringNumberEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "ringType", outputColumnName: "ringTypeEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "sporePrintColor", outputColumnName: "sporePrintColorEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "population", outputColumnName: "populationEncoded"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(inputColumnName: "habitat", outputColumnName: "habitatEncoded"))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "capShapeEncoded",
                    "capSurfaceEncoded",
                    "capColorEncoded",
                    "bruisesEncoded",
                    "odorEncoded",
                    "gillAttachmentEncoded",
                    "gillSpacingEncoded",
                    "gillSizeEncoded",
                    "gillColorEncoded",
                    "stalkShapeEncoded",
                    "stalkRootEncoded",
                    "stalkSurfaceAboveRingEncoded",
                    "stalkSurfaceBelowRingEncoded",
                    "stalkColorAboveRingEncoded",
                    "stalkColorBelowRingEncoded",
                    "veilTypeEncoded",
                    "veilColorEncoded",
                    "ringNumberEncoded",
                    "ringTypeEncoded",
                    "sporePrintColorEncoded",
                    "populationEncoded",
                    "habitatEncoded"
                ))
                .AppendCacheCheckpoint(_mlContext);

            return pipeline;
        }
        static IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            _trainedModel = trainingPipeline.Fit(trainingDataView);
            _predEngine = _mlContext.Model.CreatePredictionEngine<Mushroom, MushroomPrediction>(_trainedModel);
            return trainingPipeline;


        }
        static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            mlContext.Model.Save(model, trainingDataViewSchema, _modelPath);

            Console.WriteLine("The model is saved to {0}", _modelPath);
        }

        public MushroomPrediction Predict(Mushroom data)
        {
            return _predEngine.Predict(data);
        }

        static void AppendNewDataToDataset(Mushroom mushroom)
        {
            try
            {
                string[] rowData =
        [
            mushroom.edibility,
            mushroom.capShape,
            mushroom.capSurface,
            mushroom.capColor,
            mushroom.bruises,
            mushroom.odor,
            mushroom.gillAttachment,
            mushroom.gillSpacing,
            mushroom.gillSize,
            mushroom.gillColor,
            mushroom.stalkShape,
            mushroom.stalkRoot,
            mushroom.stalkSurfaceAboveRing,
            mushroom.stalkSurfaceBelowRing,
            mushroom.stalkColorAboveRing,
            mushroom.stalkColorBelowRing,
            mushroom.veilType,
            mushroom.veilColor,
            mushroom.ringNumber,
            mushroom.ringType,
            mushroom.sporePrintColor,
            mushroom.population,
            mushroom.habitat
        ];
                string rowToAdd = string.Join("\t", rowData);

                using (StreamWriter writer = new StreamWriter(_trainDataPath, append: true))
                {
                    writer.WriteLine(rowToAdd);
                }

                Console.WriteLine("Mushroom data added to TSV file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public string UpdateModel(Mushroom data)
        {
            try
            {
                AppendNewDataToDataset(data);
                _trainingDataView = _mlContext.Data.LoadFromTextFile<Mushroom>(_trainDataPath, hasHeader: true);
                TrainModel();
                return "Model was updated successfully.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
            
        }   
    }
}
