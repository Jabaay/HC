using System;
using UnityEngine;
using UnityEngine.UI;

public class HammingCoding : MonoBehaviour
{
    public int numOfBits; // number of bits for transmission
    public int[] originalData; // original data of 4 bits
    int[] currentEncodedData; // encoded word
    int[] decodedData; // Decode the received data

    // Channel 1 parameters
    float errorPChannel1; // error probability for channel 1
    private int errorBitsChannel1;
    private int errorCorrectionCountChannel1;

    // Channel 2 parameters
    float goodStateP = 0.9f;
    float badStateP = 0.1f;
    float good2BadTP = 0.1f; // good to bad transition probability
    float bad2GoodTP = 0.6f; // bad to good transition probability
    float goodStateEP = 0.01f; // error probability of good state
    float badStateEP = 0.1f; // error probability of bad state
    private int errorBitsChannel2;
    private int errorCorrectionCountChannel2;

    // Instances for display
    public int totalBits;
    public float berChannel1; // bit error rate for channel 1
    public float ecrChannel1; // error correction rate for channel 1
    public float berChannel2; // bit error rate for channel 2
    public float ecrChannel2; // error correction rate for channel 2

    bool channelState; // true for good, false for bad

    [SerializeField] private UIManager uim;

    void Start()
    {
        // Initialize the instances
        numOfBits = UnityEngine.Random.Range(10000, 50001);
        Debug.Log("Number of Bits: " + numOfBits);
        uim.numOfBitsText.text = "Number of Bits Generated: " + numOfBits;
        originalData = new int[4];

        totalBits = 0;
        errorBitsChannel1 = 0;
        errorCorrectionCountChannel1 = 0;
        errorBitsChannel2 = 0;
        errorCorrectionCountChannel2 = 0;
        channelState = true; // Start in the good state
    }

    void Update()
    {
        // Run simulations for both channels
        SimulateChannel1();
        SimulateChannel2();
    }

    // Simulate Channel Model 1
    void SimulateChannel1()
    {
        // Set the initial range of error probabilities for Channel Model 1
        float[] errorProbabilitiesModel1 = { 0.1f };

        foreach (float errorP in errorProbabilitiesModel1)
        {
            uim.errorPText.text = "Error Probablity: " + errorP;
            // Run the simulation for Channel Model 1
            RunSimulationModel1(errorP);
        }
    }

    // Simulate Channel Model 2
    void SimulateChannel2()
    {
        for (int i = 0; i < numOfBits; i += 4)
        {
            totalBits += 4;

            // Check channel state and transition accordingly
            if (channelState && UnityEngine.Random.Range(0f, 1f) < good2BadTP)
            {
                channelState = UnityEngine.Random.Range(0f, 1f) < goodStateP ? false : true; // transition to bad state
            }
            else if (!channelState && UnityEngine.Random.Range(0f, 1f) < bad2GoodTP)
            {
                channelState = UnityEngine.Random.Range(0f, 1f) < badStateP ? true : false; // transition to good state
            }

            int errorPosition = UnityEngine.Random.Range(0, 4); // choose error position, since it's 4-bit

            // Choose error probability based on the channel state
            float currentErrorProbability = channelState ? goodStateEP : badStateEP;

            // Store the original data before any modification
            int[] originalDataCopy = new int[4];
            Array.Copy(originalData, originalDataCopy, 4);

            for (int j = 0; j < 4; j++)
            {
                originalData[j] = UnityEngine.Random.Range(0, 2);

                // set errors based on the channel state and error probability
                if (j == errorPosition && UnityEngine.Random.Range(0f, 1f) < currentErrorProbability)
                {
                    originalData[j] = 1 - originalData[j]; // flip the bit
                }
            }

            int[] currentEncodedData = HammingEncoder(originalDataCopy);
            int[] decodedData = HammingDecoder(currentEncodedData);

            // check if an error occurred and was successfully corrected
            bool errorOccurred = false;
            for (int j = 0; j < 4; j++)
            {
                if (originalDataCopy[j] != decodedData[j])
                {
                    errorOccurred = true; // error detected
                    errorBitsChannel2++; // count the number of error bits
                    break;
                }
            }

            if (!errorOccurred)
            {
                errorCorrectionCountChannel2++; // increment if no error or successfully corrected
            }
        }

        // calculate and display BER and Error Correction Rate for Channel 2
        berChannel2 = (float)errorBitsChannel2 / totalBits;
        ecrChannel2 = (float)errorCorrectionCountChannel2 / totalBits;

        uim.berText2.text = "Channel 2 - Bit Error Rate (BER): " + berChannel2 * 100 + " %";
        uim.errorCorrectedText.text = "Channel 2 - Errors Corrected: " + (int)(numOfBits * ecrChannel2);
        uim.ecrText2.text = "Channel 2 - Error Correction Rate: " + ecrChannel2 * 100 + " %";
    }

    // Run the simulation for Channel Model 1
    void RunSimulationModel1(float errorP)
    {
        errorPChannel1 = errorP;

        for (int i = 0; i < numOfBits; i += 4)
        {
            totalBits += 4;

            // Store the original data before any modification
            int[] originalDataCopy = new int[4];
            Array.Copy(originalData, originalDataCopy, 4);

            for (int j = 0; j < 4; j++)
            {
                originalData[j] = UnityEngine.Random.Range(0, 2);

                // set errors based on the error probability
                if (UnityEngine.Random.Range(0f, 1f) < errorPChannel1)
                {
                    originalData[j] = 1 - originalData[j]; // flip the bit
                }
            }

            int[] currentEncodedData = HammingEncoder(originalDataCopy);
            int[] decodedData = HammingDecoder(currentEncodedData);

            // check if an error occurred and was successfully corrected
            bool errorOccurred = false;
            for (int j = 0; j < 4; j++)
            {
                if (originalDataCopy[j] != decodedData[j])
                {
                    errorOccurred = true; // error detected
                    errorBitsChannel1++; // count the number of error bits
                    break;
                }
            }

            if (!errorOccurred)
            {
                errorCorrectionCountChannel1++; // increment if no error or successfully corrected
            }
        }

        // calculate and display BER and Error Correction Rate for Channel 1
        berChannel1 = (float)errorBitsChannel1 / totalBits;
        ecrChannel1 = (float)errorCorrectionCountChannel1 / totalBits;

        uim.berText1.text = "Channel 1 - Bit Error Rate (BER): " + berChannel1 * 100 + " %";
        uim.errorCorrectedText.text = "Channel 1 - Errors Corrected: " + (int)(numOfBits * ecrChannel1);
        uim.ecrText1.text = "Channel 1 - Error Correction Rate: " + ecrChannel1 * 100 + " %";
    }

    // The encoding process.
    int[] HammingEncoder(int[] data)
    {
        int[,] encodingMatrix = new int[,]
        {
        {1, 0, 0, 0, 1, 1, 1},
        {0, 1, 0, 0, 1, 1, 0},
        {0, 0, 1, 0, 1, 0, 1},
        {0, 0, 0, 1, 0, 1, 1}
        };

        int[] encodedData = new int[7];

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                encodedData[j] += data[i] * encodingMatrix[i, j];
            }
        }

        for (int j = 0; j < 7; j++)
        {
            encodedData[j] %= 2; // mod 2 to get binary values
        }

        return encodedData;
    }

    // The decoding process.
    int[] HammingDecoder(int[] receivedData)
    {
        int[,] decodingMatrix = new int[,]
        {
        {1, 1, 1, 0, 1, 0, 0},
        {1, 0, 1, 1, 0, 1, 0},
        {0, 1, 1, 1, 0, 0, 1}
        };

        int[] syndrome = new int[3];

        // calculate syndrome
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                syndrome[i] += receivedData[j] * decodingMatrix[i, j];
            }
            syndrome[i] %= 2; // mod 2
        }

        // find the error position
        int errorPosition = syndrome[0] + 2 * syndrome[1] + 4 * syndrome[2];

        // correct the error if found
        if (errorPosition > 0)
        {
            receivedData[errorPosition - 1] = 1 - receivedData[errorPosition - 1];
        }

        // return the decoded data (without parity bits)
        int[] decodedData = new int[4];
        for (int i = 0, j = 0; i < 7; i++)
        {
            if (i != 0 && i != 1 && i != 3)
            {
                decodedData[j++] = receivedData[i];
            }
        }

        return decodedData;

    }


}