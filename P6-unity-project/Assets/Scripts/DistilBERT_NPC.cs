using UnityEngine;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.Generic;
using System.Linq;

public class DistilBERT_NPC : MonoBehaviour
{
    private InferenceSession session;

    // Simple word-to-index dictionary for tokenization (Example only)
    private Dictionary<string, long> wordToIndex = new Dictionary<string, long>()
    {
        { "[CLS]", 101 }, { "[SEP]", 102 }, { "hello", 7592 }, { "world", 2088 }, { "who", 2040 }, { "are", 2024 }, { "you", 2017 }
        // Add more words or use a real tokenizer (this is a placeholder)
    };

    void Start()
    {
        string modelPath = Application.dataPath + "/AI_Models/model.onnx";
        session = new InferenceSession(modelPath);
        Debug.Log("DistilBERT Model Loaded Successfully!");
    }

    public string GetNPCResponse(string inputText)
    {
        // Convert text to tokens
        var tokenizedInput = TokenizeText(inputText);

        // Convert to tensor format
        var inputTensor = new DenseTensor<long>(tokenizedInput, new int[] { 1, tokenizedInput.Length });

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputTensor)
        };

        // Run inference
        using (var results = session.Run(inputs))
        {
            var outputTensor = results.First().AsTensor<float>();
            return InterpretModelOutput(outputTensor);
        }
    }

    private long[] TokenizeText(string text)
    {
        List<long> tokens = new List<long> { 101 }; // Start token [CLS]

        foreach (string word in text.ToLower().Split(' '))
        {
            if (wordToIndex.TryGetValue(word, out long token))
            {
                tokens.Add(token);
            }
            else
            {
                tokens.Add(100); // Unknown token
            }
        }

        tokens.Add(102); // End token [SEP]

        return tokens.ToArray();
    }

    private string InterpretModelOutput(Tensor<float> outputTensor)
    {
        // Convert AI output into a readable response (Placeholder)
        return "NPC: That sounds interesting!";
    }
}
