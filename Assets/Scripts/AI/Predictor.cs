using Unity.Sentis;
using UnityEngine;

public class Predictor : MonoBehaviour
{
    public ModelAsset modelAsset;
    Model runtimeModel;
    IWorker worker;
    
    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
    }
    
    public float[] Predict(TensorFloat inputTensor)
    {
        worker.Execute(inputTensor);
        var outputTensor = worker.PeekOutput() as TensorFloat;
        
        outputTensor.CompleteOperationsAndDownload();
        float[] outputArray = outputTensor.ToReadOnlyArray();
        
        return outputArray;
    }
    
    void OnDestroy()
    {
        worker.Dispose();
    }
}