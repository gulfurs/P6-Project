using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro support
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.Generic;
using System.Linq;

public class DistilBert_NPC : MonoBehaviour
{
    public TMP_InputField playerInputField; // Player input field
    public TMP_Text npcResponseText; // NPC response text

    private InferenceSession session;

    void Start()
    {
        string modelPath = Application.dataPath + "/AI_Models/gpt2-10.onnx";
        session = new InferenceSession(modelPath);
        Debug.Log("AI Model Loaded Successfully!");
    }

    public void OnPlayerSubmit()
    {
        string playerText = playerInputField.text;
        string npcResponse = GetNPCResponse(playerText);
        npcResponseText.text = npcResponse;
    }

    private string GetNPCResponse(string inputText)
    {
        var tokenizedInput = TokenizeText(inputText);
        int seqLength = tokenizedInput.Length;

        var inputTensor = new DenseTensor<long>(tokenizedInput, new int[] { 1, seqLength });

        // Create attention mask (1 for real tokens, 0 for padding)
        var attentionMask = new DenseTensor<long>(Enumerable.Repeat(1L, seqLength).ToArray(), new int[] { 1, seqLength });

        // Create token type IDs (0 for single-sentence inputs)
        var tokenTypeIds = new DenseTensor<long>(new long[seqLength], new int[] { 1, seqLength });

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputTensor),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMask),
            NamedOnnxValue.CreateFromTensor("token_type_ids", tokenTypeIds)
        };

        using (var results = session.Run(inputs))
        {
            var outputTensor = results.First().AsTensor<float>();
            return InterpretModelOutput(outputTensor);
        }
    }

    // Tokenization function - if you're using GPT-2, remove the manual dictionary
    private long[] TokenizeText(string text)
    {
        // The tokenizer will handle this, but for illustration:
        // For GPT-2, you might need pre-tokenized input or handle tokenization externally in Python
        return TokenizeGPT2Text(text);  // Replace with real tokenization logic
    }

    private long[] TokenizeGPT2Text(string text)
    {
        // You would handle tokenization here for GPT-2
        // If running in Unity, you may need to pre-tokenize text externally
        return new long[] { 101, 7592, 2088, 102 }; // Example tokens for "hello world"
    }

    private string InterpretModelOutput(Tensor<float> outputTensor)
    {
        // Convert model output tensor to human-readable response
        return "NPC: That sounds interesting!"; // Placeholder response
    }
}
