using UnityEngine;
using System.Diagnostics;
using System.IO;

public class LlamaRunner : MonoBehaviour
{
    private string modelPath;
    private string llamaPath;

    void Start()
    {
        modelPath = Path.Combine(Application.dataPath, "AI_Models/mistral-7b-v0.1.Q4_K_M.gguf");
        llamaPath = Path.Combine(Application.dataPath, "AI_Models/llama-simple.exe");  // Use `llama-simple-chat.exe` for chatbot

        if (!File.Exists(modelPath) || !File.Exists(llamaPath))
        {
            UnityEngine.Debug.LogError("Llama model or executable not found!");
            return;
        }

        string response = RunLlama("Hello! How are you?");
        UnityEngine.Debug.Log("AI Response: " + response);
    }

    public string RunLlama(string prompt)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = llamaPath,
            Arguments = $"-m \"{modelPath}\" -p \"{prompt}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process { StartInfo = psi };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return result;
    }
}


