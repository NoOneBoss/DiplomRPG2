using Player;
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
        var rootElement = _UIDocument.rootVisualElement;
        _playerName = rootElement.Q<Label>(name = "playerName");
        _healthBar = rootElement.Q<ProgressBar>(name = "playerHealth");
        _staminaBar = rootElement.Q<ProgressBar>(name = "playerStamina");
        
        //Name
        _playerName.SetBinding(nameof(Label.text), new DataBinding()
        {
            bindingMode = BindingMode.ToTargetOnce,
            dataSource = PlayerManager.Singleton.playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.playerName))
        });
        
        
        //Health
        _healthBar.SetBinding(nameof(ProgressBar.value), new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = PlayerManager.Singleton.playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.currentHealth))
        });
        _healthBar.SetBinding(nameof(ProgressBar.highValue), new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = PlayerManager.Singleton.playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.maxHealth))
        });


        var healthBinding = new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = PlayerManager.Singleton.playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.currentHealth))
        };
        if (ConverterGroups.TryGetConverterGroup("Health Converter", out var group))
        {
            healthBinding.ApplyConverterGroupToUI(group);
            healthBinding.ApplyConverterGroupToSource(group);
        }
        _healthBar.SetBinding(nameof(ProgressBar.title), healthBinding);
        
        //Stamina
        _staminaBar.SetBinding(nameof(ProgressBar.value), new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = PlayerManager.Singleton.playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.currentStamina))
        });
        _staminaBar.SetBinding(nameof(ProgressBar.highValue), new DataBinding()
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = PlayerManager.Singleton.playerData,
            dataSourcePath = PropertyPath.FromName(nameof(PlayerData.maxStamina))
        });
    }
}
