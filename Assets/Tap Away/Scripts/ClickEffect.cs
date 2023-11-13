using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEffect : MonoBehaviour
{
    public static ClickEffect Instance;

    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Camera _camera;
    [SerializeField] private float distanceFromCamera = 10f;
    private ParticleSystem[] _particleSystems;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        string tapName = ShopReadWriteData.Instance.GetEquippedEquipmentName("TapEffect");

        if (ShopManager.Instance.GetTapSO(tapName).ShopItemPrefab != null)
            _particleSystem = ShopManager.Instance.GetTapSO(tapName).ShopItemPrefab.GetComponent<ParticleSystem>();

        _particleSystem = Instantiate(_particleSystem);
        _particleSystems = _particleSystem.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in _particleSystems)
        {
            var mainModule = ps.main;
            mainModule.loop = false;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceFromCamera; // Depth from the camera in world units
            Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);
            _particleSystem.transform.position = worldPos;
            _particleSystem.Play();
     
        }
    }

    public void ChangeEffect(ParticleSystem newEffect)
    {
        Destroy(_particleSystem);
        _particleSystem = Instantiate(newEffect);
        _particleSystems = _particleSystem.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in _particleSystems)
        {
            var mainModule = ps.main;
            mainModule.loop = false;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }
}
