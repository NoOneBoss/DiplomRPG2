using System.Linq;
using Player;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController Singleton;
    
    public UIDocument _UIDocument;
    private Label _playerName;
    private ProgressBar _healthBar;
    private ProgressBar _staminaBar;

    private void Start()
    {
        Singleton = this;
    }

    public void StartUI()
    {
        registerConverters();
        
        var rootElement = _UIDocument.rootVisualElement;
        _playerName = rootElement.Q<Label>(name = "playerName");
        _healthBar = rootElement.Q<ProgressBar>(name = "playerHealth");
        _staminaBar = rootElement.Q<ProgressBar>(name = "playerStamina");

        PlayerData playerData = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<PlayerDataHandler>().playerData;
        
        
        //Name
        _playerName.SetBinding(nameof(Label.text), new DataBinding()
        {
            bindingMode = BindingMode.ToTargetOnce,
            dataSource = playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.playerName))
        });
        
        
        //Health
        var healthBinding = new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.currentHealth))
        };
        var maxHealthBinding = new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.maxHealth))
        };
        if (ConverterGroups.TryGetConverterGroup("Health Converter", out var group))
        {
            healthBinding.ApplyConverterGroupToUI(group);
            healthBinding.ApplyConverterGroupToSource(group);
        }
        _healthBar.SetBinding(nameof(ProgressBar.value), healthBinding);
        _healthBar.SetBinding(nameof(ProgressBar.title), healthBinding);
        _healthBar.SetBinding(nameof(ProgressBar.highValue), maxHealthBinding);
        
        //Stamina
        _staminaBar.SetBinding(nameof(ProgressBar.value), new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.currentStamina))
        });
        _staminaBar.SetBinding(nameof(ProgressBar.highValue), new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.maxStamina))
        });
    }

    private void registerConverters()
    {
        var group = new ConverterGroup("Health Converter");
        
        PlayerData playerData = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<PlayerDataHandler>().playerData;
        group.AddConverter((ref int v) => $"{v}/{playerData.maxHealth} ХП");
        group.AddConverter((ref float v) => $"{v}/{playerData.maxHealth} ХП");
        
        ConverterGroups.RegisterConverterGroup(group);
    }
}
