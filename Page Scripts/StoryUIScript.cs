using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class StoryUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        SetSoryUI();
    }

    void SetSoryUI()
    {
        string[] stories = {
            "In the midst of final exams week at PDM, a mysterious virus starts spreading among the students. At first, it’s dismissed as a common flu, but soon, those infected begin to exhibit bizarre behavior and an insatiable hunger for human flesh.",
            "As panic grips the campus, a group of unlikely heroes emerges: a shy computer science student, a resourceful cafeteria worker, a tough-as-nails security guard, and a brilliant but eccentric biology professor. Together, they must navigate the school grounds of the campus to find answers about the virus and a way to stop it.",
            "As they uncover the truth, they realize that the virus was engineered by (name of the boss), a rogue scientist who used to be a faculty member at the campus. With time running out and the infected multiplying, you as a student of the campus must race against the clock to  defeat the final boss, find an antidote and save your fellow students.",
            "The story explores themes of survival, flexibility, and the resilience of the human spirit in the face of a terrifying and unexpected crisis. Will you manage to contain the outbreak and restore order to your campus, or will you become just another meal for the Campus of the Undead",
        };

        VisualElement storyRoot = root.Q<VisualElement>("StoryRoot");

        Label storyLabel = root.Q<Label>("StoryLabel");
        Label storyIndexLabel = root.Q<Label>("StoryIndexLabel");

        Button prevButton = root.Q<Button>("PrevButton");
        Button nextButton = root.Q<Button>("NextButton");
        Button skipButton = root.Q<Button>("SkipButton");

        int storyIndex = 0;

        void DisplayStory(int index)
        {
            storyLabel.text = stories[index];
            storyIndexLabel.text = (index + 1) + "/" + stories.Length;
            prevButton.SetEnabled(!(index == 0));
            // nextButton.SetEnabled(!(index == stories.Length - 1));

            if (index == stories.Length - 1)
            {
                nextButton.text = "Start game";
                skipButton.SetEnabled(false);
                nextButton.clickable = new Clickable(() =>
                {
                    nextButton.SetEnabled(false);
                    LoaderUtility.Initialize();
                    SceneManager.LoadSceneAsync("ARScene", LoadSceneMode.Single);
                });
            }
            else
            {
                nextButton.text = "Next";
                nextButton.clickable = new Clickable(() =>
                {
                    storyIndex += 1;
                    DisplayStory(storyIndex);
                });
            }
        }

        DisplayStory(storyIndex);

        storyRoot.AddManipulator(new Clickable((e) =>
        {

            if (storyIndex != stories.Length - 1)
            {
                storyIndex += 1;
                DisplayStory(storyIndex);

                LoaderUtility.Initialize();
                SceneManager.LoadSceneAsync("ARScene", LoadSceneMode.Single);
            }
        }));

        prevButton.clickable = new Clickable(() =>
        {
            storyIndex -= 1;
            DisplayStory(storyIndex);
        });

        nextButton.clickable = new Clickable(() =>
        {
            storyIndex += 1;
            DisplayStory(storyIndex);
        });

        skipButton.clickable = new Clickable(() =>
        {
            LoaderUtility.Initialize();
            SceneManager.LoadSceneAsync("ARScene", LoadSceneMode.Single);

            skipButton.SetEnabled(false);
        });

    }

    // Update is called once per frame
}
