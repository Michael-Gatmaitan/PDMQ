using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    public string selectedCharacter = "";
    void Start()
    {
        Celestia celestiaClass = new();
        Nightshade nightshadeClass = new();

        (string skillTitle, string desc, string path)[] celestiaSkills = celestiaClass.celestiaSkills;
        (string name, string desc) celestia = celestiaClass.celestia;

        (string skillTitle, string desc, string path)[] nightshadeSkills = nightshadeClass.nightshadeSkills;
        (string name, string desc) nightshade = nightshadeClass.nightshade;

        root = gameObject.GetComponent<UIDocument>().rootVisualElement;

        Button ch1 = root.Q<Button>("CH1");
        Button ch2 = root.Q<Button>("CH2");

        Button viewInfo = root.Q<Button>("ViewInfo");

        VisualElement characterDetailsContainer = root.Q<VisualElement>("CharacterDetails");
        Button closeCharacterDetails = root.Q<Button>("CloseChDetails");
        closeCharacterDetails.clickable = new Clickable(() => characterDetailsContainer.style.display = DisplayStyle.None);

        Label chNameLabel = root.Q<Label>("CharacterName");
        Label chDescLabel = root.Q<Label>("CharacterDescription");
        VisualElement chAboutDispBox = root.Q<VisualElement>("CHAboutDispBox");

        Button[] skillButtons = { root.Q<Button>("Skill1"), root.Q<Button>("Skill2"), root.Q<Button>("Skill3"), root.Q<Button>("Skill4") };

        Label skillTitleLabel = root.Q<Label>("SkillTitle");
        Label skillDescriptionLabel = root.Q<Label>("SkillDescription");

        Button selectCharacterButton = root.Q<Button>("SelectCharacter");

        Button closeButton = root.Q<Button>("CloseButton");

        CharacterButtonClickables();

        SetCloseButton();

        void CharacterButtonClickables()
        {
            ch1.clickable = new Clickable(() =>
            {
                ch1.style.backgroundColor = new Color(0.20f, .20f, .20f);
                ch2.style.backgroundColor = new Color(0.94f, .73f, .18f);
                selectedCharacter = "nightshade";

                viewInfo.style.visibility = Visibility.Visible;

                viewInfo.clickable = new Clickable(() =>
                {
                    ShowCharacterInfo("nightshade");
                });
            });

            ch2.clickable = new Clickable(() =>
            {
                ch1.style.backgroundColor = new Color(0.94f, .73f, .18f);
                ch2.style.backgroundColor = new Color(0.20f, .20f, .20f);
                selectedCharacter = "celestia";

                viewInfo.style.visibility = Visibility.Visible;

                viewInfo.clickable = new Clickable(() =>
                {
                    ShowCharacterInfo("celestia");
                });
            });

            void ShowCharacterInfo(string chName)
            {
                Debug.Log("Showing " + chName + " ...");
                if (!(chName == "nightshade" || chName == "celestia"))
                {
                    Debug.LogError("Character doesn't exist.");
                    return;
                }

                // Show the character's details based on character selected to view.
                characterDetailsContainer.style.display = DisplayStyle.Flex;

                chNameLabel.text = chName == "nightshade" ? nightshade.name : celestia.name;
                chDescLabel.text = chName == "nightshade" ? nightshade.desc : celestia.desc;

                String chSpritePath = chName == "nightshade" ? "ChooseCharacters/nightshade" : "ChooseCharacters/celestia";
                Sprite chSelectedSprite = Resources.Load<Sprite>(chSpritePath);

                chAboutDispBox.style.backgroundImage = new StyleBackground(chSelectedSprite);

                for (int i = 0; i < skillButtons.Length; i++)
                {
                    (string skillTitle, string desc, string path) skill = chName == "nightshade" ? nightshadeSkills[i] : celestiaSkills[i];

                    Sprite skillSprite = Resources.Load<Sprite>(skill.path);
                    skillButtons[i].style.backgroundImage = new StyleBackground(skillSprite);

                    Debug.Log(skillSprite);

                    int index = i;
                    skillButtons[i].clickable = new Clickable(() =>
                    {
                        skillTitleLabel.text = skill.skillTitle;
                        skillDescriptionLabel.text = skill.desc;

                        for (int x = 0; x < skillButtons.Length; x++)
                        {
                            Color borderColor = index == x ? new(1, 0, 1) : new(1, 0, 0);
                            skillButtons[x].style.borderTopColor = borderColor;
                            skillButtons[x].style.borderRightColor = borderColor;
                            skillButtons[x].style.borderBottomColor = borderColor;
                            skillButtons[x].style.borderLeftColor = borderColor;
                        }
                    });

                    // Disalbe button
                    // skillButtons[i].style.display = DisplayStyle.None;
                }
                // Set save player preferences on click base on selected character.
                selectCharacterButton.clickable = new Clickable(() =>
                {
                    PlayerPrefs.SetString("choosenCharacter", chName);
                    SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
                });
            }
        }
        // Close the current scene and back to the 'MainScene' scene.
        void SetCloseButton()
        {
            closeButton.clickable = new Clickable(() =>
            {
                SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            });
        }
    }
}
