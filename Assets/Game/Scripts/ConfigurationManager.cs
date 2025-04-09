using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private GameObject lensPrefab;
    [SerializeField] private GameObject mirrorPrefab;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private string saveFolder = "OpticalConfigs";
    [SerializeField] private FileDialog fileDialog;
    
    private string saveDirectory;

    private void Awake()
    {
        saveDirectory = Path.Combine(Application.persistentDataPath, saveFolder);
        
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        saveButton.onClick.AddListener(ShowSaveDialog);
        loadButton.onClick.AddListener(ShowLoadDialog);
    }

    private void ShowSaveDialog()
    {
        string path = fileDialog.ShowDialog(
            "Save Configuration", 
            saveDirectory, 
            "opt", 
            FileDialog.Mode.Save);

        if (!string.IsNullOrEmpty(path))
        {
            if (!path.EndsWith(".opt"))
            {
                path += ".opt";
            }
            SaveConfiguration(path);
        }
    }

    private void ShowLoadDialog()
    {
        string path = fileDialog.ShowDialog(
            "Load Configuration", 
            saveDirectory, 
            "opt", 
            FileDialog.Mode.Open);

        if (!string.IsNullOrEmpty(path))
        {
            LoadConfiguration(path);
        }
    }

    private void SaveConfiguration(string filePath)
    {
        var config = new OpticalConfiguration();
        
        var lenses = GameObject.FindGameObjectsWithTag("LensContainer");
        var mirrors = GameObject.FindGameObjectsWithTag("Mirror");
        var lasers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var lens in lenses)
        {
            config.lenses.Add(LensData.FromLens(lens));
        }

        foreach (var mirror in mirrors)
        {
            config.mirrors.Add(MirrorData.FromMirror(mirror));
        }
        foreach (var laser in lasers)
        {
            config.lasers.Add(LaserData.FromLaser(laser));
        }

        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);

        Debug.Log($"Configuration saved to {filePath}");
    }

    private void LoadConfiguration(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Configuration file not found: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        OpticalConfiguration config = JsonUtility.FromJson<OpticalConfiguration>(json);
        
        ClearExistingObjects();

        foreach (var lensData in config.lenses)
        {
            var lens = Instantiate(lensPrefab, Vector3.zero, Quaternion.identity);
            lensData.ApplyTo(lens);
        }

        foreach (var mirrorData in config.mirrors)
        {
            var mirror = Instantiate(mirrorPrefab, Vector3.zero, Quaternion.identity);
            mirrorData.ApplyTo(mirror);
        }
        
        foreach (var laserData in config.lasers)
        {
            var laser = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
            laserData.ApplyTo(laser);
        }

        Debug.Log($"Configuration loaded from {filePath}");
    }

    private void ClearExistingObjects()
    {
        var existingLenses = GameObject.FindGameObjectsWithTag("LensContainer");
        var existingMirrors = GameObject.FindGameObjectsWithTag("Mirror");
        var existiongLasers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in existingLenses)
        {
            Destroy(obj.gameObject);
        }
        
        foreach (var obj in existingMirrors)
        {
            Destroy(obj.gameObject);
        }
        foreach (var obj in existiongLasers)
        {
            Destroy(obj.gameObject);
        }
    }
}

[System.Serializable]
public class OpticalConfiguration
{
    public List<LensData> lenses = new List<LensData>();
    public List<MirrorData> mirrors = new List<MirrorData>();
    public List<LaserData> lasers = new List<LaserData>();
}

[System.Serializable]
public class LensData
{
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    public float scaleX, scaleY, scaleZ;
    public float focus;
    public float refractiveIndex;

    public static LensData FromLens(GameObject lens)
    {
        var lensComp = lens.GetComponentInChildren<Lens>();
        return new LensData
        {
            posX = lens.transform.position.x,
            posY = lens.transform.position.y,
            posZ = lens.transform.position.z,
            rotX = lens.transform.rotation.x,
            rotY = lens.transform.rotation.y,
            rotZ = lens.transform.rotation.z,
            rotW = lens.transform.rotation.w,
            scaleX = lens.transform.localScale.x,
            scaleY = lens.transform.localScale.y,
            scaleZ = lens.transform.localScale.z,
            focus = lensComp?.Focus ?? 0f,
            refractiveIndex = lensComp?.RefractiveIndex ?? 1.5f
        };
    }

    public void ApplyTo(GameObject lens)
    {
        lens.transform.position = new Vector3(posX, posY, posZ);
        lens.transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
        lens.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        
        var lensComp = lens.GetComponentInChildren<Lens>();
        if (lensComp != null)
        {
            lensComp.Focus = focus;
            lensComp.RefractiveIndex = refractiveIndex;
        }
    }
}

[System.Serializable]
public class MirrorData
{
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    public float scaleX, scaleY, scaleZ;

    public static MirrorData FromMirror(GameObject mirror)
    {
        return new MirrorData
        {
            posX = mirror.transform.position.x,
            posY = mirror.transform.position.y,
            posZ = mirror.transform.position.z,
            rotX = mirror.transform.rotation.x,
            rotY = mirror.transform.rotation.y,
            rotZ = mirror.transform.rotation.z,
            rotW = mirror.transform.rotation.w,
            scaleX = mirror.transform.localScale.x,
            scaleY = mirror.transform.localScale.y,
            scaleZ = mirror.transform.localScale.z
        };
    }

    public void ApplyTo(GameObject mirror)
    {
        mirror.transform.position = new Vector3(posX, posY, posZ);
        mirror.transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
        mirror.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}

[System.Serializable]
public class LaserData
{
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    public float scaleX, scaleY, scaleZ;

    public static LaserData FromLaser(GameObject laser)
    {
        return new LaserData
        {
            posX = laser.transform.position.x,
            posY = laser.transform.position.y,
            posZ = laser.transform.position.z,
            rotX = laser.transform.rotation.x,
            rotY = laser.transform.rotation.y,
            rotZ = laser.transform.rotation.z,
            rotW = laser.transform.rotation.w,
            scaleX = laser.transform.localScale.x,
            scaleY = laser.transform.localScale.y,
            scaleZ = laser.transform.localScale.z
        };
    }

    public void ApplyTo(GameObject laser)
    {
        laser.transform.position = new Vector3(posX, posY, posZ);
        laser.transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
        laser.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}