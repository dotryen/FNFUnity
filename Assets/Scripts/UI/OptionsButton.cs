using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButton : MenuOption {
    // public MenuOption applyButton;
    public MenuOption sectionTemplate;
    public SettingButton buttonTemplate;

    private Dictionary<string, MenuOption> existingSections = new Dictionary<string, MenuOption>();

    protected override void Awake() {
        var settings = Settings.GetSettings();
        foreach (var pair in settings) {
            if (!existingSections.TryGetValue(pair.Value.sectionName, out var section)) {
                section = Instantiate(sectionTemplate, submenuTrans);

                section.defaultText = pair.Value.sectionName;
                section.description = pair.Value.sectionDescription;

                existingSections.Add(pair.Value.sectionName, section);
            }

            var button = Instantiate(buttonTemplate, section.submenuTrans);

            button.defaultText = pair.Value.displayName;
            button.description = pair.Value.description;
            button.settingKey = pair.Key;

            button.gameObject.SetActive(true);
        }

        foreach (var pair in existingSections) {
            pair.Value.gameObject.SetActive(true);
        }

        // applyButton.origTransform.SetAsLastSibling();
        SettingsController.Initialize();

        base.Awake();
    }
}
