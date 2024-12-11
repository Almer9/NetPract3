using MLTEST;
using NetPract3.Model;
namespace NetPract3.Service
{
    public class ModelService
    {
        private readonly MushroomModelTrainer _modelTrainer;

        public ModelService(MushroomModelTrainer modelTrainer)
        {
            _modelTrainer = modelTrainer;
        }
        public string Train()
        {
            return _modelTrainer.LoadModel();
        }

        public string Predict(Mushroom data)
        {
            MushroomPrediction prediction = _modelTrainer.Predict(data);
            return prediction.edibility;
        }

        public void Retrain(Mushroom data)
        {
            _modelTrainer.UpdateModel(data);
        }
    }
}
