using System.Collections.Generic;
using UnityEngine;

public class DataRecorder : MonoBehaviour
{
    [System.Serializable]
    public struct Data
    {
        public string objectID;
        public Vector3 position;
        public Quaternion orientation;

        public Data(string id, Vector3 pos, Quaternion rot)
        {
            objectID = id;
            position = pos;
            orientation = rot;
        }
    }

    public List<Data> recordedData;

    private void Awake()
    {
        // Initialize with exhibit IDs matching museum_data.json
        recordedData = new List<Data>
        {
            // Storage devices
            new Data("exhibit_001", new Vector3(1.33f, 6.79f, 1.58f), Quaternion.Euler(0f, 0f, 0f)),  // Flash Memory
            new Data("exhibit_002", new Vector3(0.671f, 6.79f, 1.37f), Quaternion.Euler(0f, 0f, 0f)), // Hard Disk Drive
            new Data("exhibit_003", new Vector3(-0.18f,6.79000015f,0.84f), Quaternion.Euler(0f, 0f, 0f)), // Optical Disc
            new Data("exhibit_004", new Vector3(-0.102f, 6.79f, 0.86f), Quaternion.Euler(0f, 0f, 0f)),//Floppy Disk
            new Data("exhibit_005", new Vector3(-1.554f, 6.79f, 0.543f), Quaternion.Euler(0f, 28f, 0f)),//Removable magnetic disk
            new Data("exhibit_006", new Vector3(-2.06f, 6.79f, 0.32f), Quaternion.Euler(0f, 11.74f, 0f)),//magnetic tape
            new Data("exhibit_007", new Vector3(-3.1f, 6.79f, 0.27f), Quaternion.Euler(0f, 11.74f, 0f)),//Punched Card
            new Data("exhibit_008", new Vector3(-5.4f, 6.79f, 0.82f), Quaternion.Euler(0f, 43.75f, 0f)),//JUKI 1300 Card-punch Machine
            new Data("exhibit_009", new Vector3(-7.11f, 6.79f, 1.17f), Quaternion.Euler(0f, 32.6f, 0f)),//DEC CR-11 Card Reader
            new Data("exhibit_010", new Vector3(-8.2f, 6.79f, 0.82f), Quaternion.Euler(0f, 24.41f, 0f)),//Epson LX-800 9-Pin Dot Matrix Printer
            new Data("exhibit_011", new Vector3(-9.09f, 6.79f, 0.89f), Quaternion.Euler(0f, 24.41f, 0f)),//2nd Generation Chinese Keyboard and Input System
            new Data("exhibit_012", new Vector3(-10.55f, 6.79f, 0.65f), Quaternion.Euler(0f, 24.41f, 0f)),//Mouse
            new Data("exhibit_013", new Vector3(-11.93f, 6.79f, 0.53f), Quaternion.Euler(0f, 1.6f, 0f)),//iMac G3 Personal Computer
            new Data("exhibit_014", new Vector3(-13.41f, 6.79f, 0.4f), Quaternion.Euler(0f, 1.6f, 0f)),//Compaq Portable 386
            new Data("exhibit_015", new Vector3(-14.68f, 6.79f, 0.1f), Quaternion.Euler(0f, 1.6f, 0f)),//IBM PC-XT Personal Computer
            new Data("exhibit_016", new Vector3(-16.39f, 6.79f, 0.1f), Quaternion.Euler(0f, 28.02f, 0f)),//Apple II Personal Computer
            new Data("exhibit_017", new Vector3(2.84f, 6.79f, 0.1f), Quaternion.Euler(0f, 162.41f, 0f)),//Convex C3 mini-supercomputer
            new Data("exhibit_018", new Vector3(-0.6f, 6.79f, 0.0f), Quaternion.Euler(0f, 177.1f, 0f)),//WANG VS90 Minicomputer
            new Data("exhibit_019", new Vector3(-3.5f, 6.79f, -1.31f), Quaternion.Euler(0f, 177.1f, 0f)),  // Main CPU (center)
            new Data("exhibit_020", new Vector3(-4.8f, 6.79f, -1.31f), Quaternion.Euler(0f, 177.1f, 0f)),  // Peripheral unit (left)
            new Data("exhibit_021", new Vector3(-2.2f, 6.79f, -1.31f), Quaternion.Euler(0f, 177.1f, 0f)),  // Peripheral unit (right)
            new Data("exhibit_022", new Vector3(-10.2f, 6.79f, -1.31f), Quaternion.Euler(0f, 163.06f, 0f)),//DEC VEX Supermini Computer
            new Data("exhibit_023", new Vector3(-14.04f, 6.79f, -0.73f), Quaternion.Euler(0f, 147.4f, 0f)),//LA30P DECwriter Terminal
            new Data("exhibit_024", new Vector3(-16.3f, 6.79f, -1.8f), Quaternion.Euler(0f, 159.63f, 0f)),//PDP-11 Minicomputer
            new Data("exhibit_025", new Vector3(-18.52f, 6.79f, -1.8f), Quaternion.Euler(0f, -40.5f, 0f)),//RP03 Disk Drive
            new Data("exhibit_026", new Vector3(-19.99f, 6.79f, -2.1f), Quaternion.Euler(0f, -202.1f, 0f)),//PDP-10 Mainframe Computer
            // Camera positions for virtual museum exhibits
            new Data("camera_001", new Vector3(1.8f, 6.79f, 2.2f), Quaternion.Euler(0f, 205f, 0f)),  // Flash Memory
            new Data("camera_002", new Vector3(1.2f, 6.79f, 2.0f), Quaternion.Euler(0f, 190f, 0f)),  // Hard Disk Drive
            new Data("camera_003", new Vector3(0.4f, 6.79f, 1.6f), Quaternion.Euler(0f, 195f, 0f)),  // Floppy Disk (exhibit_003)
            new Data("camera_004", new Vector3(0.5f, 6.79f, 1.5f), Quaternion.Euler(0f, 200f, 0f)),  // Optical Disc (exhibit_004)
            new Data("camera_005", new Vector3(-0.0659999847f,6.76340246f,1.01300001f), Quaternion.Euler(0f, 210f, 0f)),  // Removable magnetic disk
            new Data("camera_006", new Vector3(-0.4416504f, 6.1673f, 1.068993f), Quaternion.Euler(0f, 274.59f, 0f)),  // Magnetic tape
            new Data("camera_007", new Vector3(-2.5f, 6.79f, 1.0f), Quaternion.Euler(0f, 195f, 0f)),  // Punched Card
            new Data("camera_008", new Vector3(-4.8f, 6.79f, 1.6f), Quaternion.Euler(0f, 225f, 0f)),  // JUKI Card-punch Machine
            new Data("camera_009", new Vector3(-6.5f, 6.79f, 2.0f), Quaternion.Euler(0f, 215f, 0f)),  // DEC Card Reader
            new Data("camera_010", new Vector3(-7.6f, 6.79f, 1.6f), Quaternion.Euler(0f, 205f, 0f)),  // Epson Printer
            new Data("camera_011", new Vector3(-8.5f, 6.79f, 1.7f), Quaternion.Euler(0f, 205f, 0f)),  // Chinese Keyboard
            new Data("camera_012", new Vector3(-10.0f, 6.79f, 1.5f), Quaternion.Euler(0f, 205f, 0f)),  // Mouse
            new Data("camera_013", new Vector3(-11.4f, 6.79f, 1.4f), Quaternion.Euler(0f, 182f, 0f)),  // iMac G3
            new Data("camera_014", new Vector3(-12.9f, 6.79f, 1.3f), Quaternion.Euler(0f, 182f, 0f)),  // Compaq Portable
            new Data("camera_015", new Vector3(-13.0f, 6.769753f, 0.348f), Quaternion.Euler(0f, -86.46f, 0f)),  // IBM PC-XT
            new Data("camera_016", new Vector3(-15.8f, 6.79f, 0.9f), Quaternion.Euler(0f, 210f, 0f)),  // Apple II
            new Data("camera_017", new Vector3(2.3f, 6.79f, -0.6f), Quaternion.Euler(0f, 340f, 0f)),  // Convex C3
            new Data("camera_018", new Vector3(-0.1f, 6.79f, -0.8f), Quaternion.Euler(0f, 355f, 0f)),  // WANG VS90
            new Data("camera_019", new Vector3(-3.0f, 6.79f, -2.1f), Quaternion.Euler(0f, 355f, 0f)),  // Main CPU (center)
            new Data("camera_020", new Vector3(-4.3f, 6.79f, -2.1f), Quaternion.Euler(0f, 355f, 0f)),  // Peripheral (left)
            new Data("camera_021", new Vector3(-1.7f, 6.79f, -2.1f), Quaternion.Euler(0f, 355f, 0f)),  // Peripheral (right)
            new Data("camera_022", new Vector3(-9.6f, 6.79f, -2.1f), Quaternion.Euler(0f, 340f, 0f)),  // DEC VEX
            new Data("camera_023", new Vector3(-13.5f, 6.79f, -1.5f), Quaternion.Euler(0f, 325f, 0f)),  // DECwriter Terminal
            new Data("camera_024", new Vector3(-15.7f, 6.79f, -2.6f), Quaternion.Euler(0f, 340f, 0f)),  // PDP-11
            new Data("camera_025", new Vector3(-17.9f, 6.79f, -2.6f), Quaternion.Euler(0f, 140f, 0f)),  // RP03 Disk Drive
            new Data("camera_026", new Vector3(-19.4f, 6.79f, -2.9f), Quaternion.Euler(0f, 20f, 0f)),  // PDP-10 Mainframe
        };

        Debug.Log("DataRecorder initialized with " + recordedData.Count + " exhibits");
    }
    public Vector3 GetPosition(string objectID)
    {
        if (recordedData == null)
        {
            Debug.LogError("recordedData is null! Make sure DataRecorder.Awake() has run.");
            return Vector3.zero;
        }

        foreach (Data data in recordedData)
        {
            if (data.objectID == objectID)
            {
                Debug.Log("Found position for " + objectID + ": " + data.position);
                return data.position;
            }
        }

        Debug.LogWarning("No position found for objectID: " + objectID);
        return Vector3.zero;
    }

    public Quaternion GetOrientation(string objectID)
    {
        if (recordedData == null)
        {
            Debug.LogError("recordedData is null! Make sure DataRecorder.Awake() has run.");
            return Quaternion.identity;
        }

        foreach (Data data in recordedData)
        {
            if (data.objectID == objectID)
            {
                Debug.Log("Found orientation for " + objectID + ": " + data.orientation);
                return data.orientation;
            }
        }

        Debug.LogWarning("No orientation found for objectID: " + objectID);
        return Quaternion.identity;
    }
}