using UnityEngine;

public class Celestia
{
    // Start is called before the first frame update
    // string nightshadeDesc = ;
    // (string skillTitle, string desc, string path) n_skill1 = ;
    // (string skillTitle, string desc, string path) n_skill2 = ;
    // (string skillTitle, string desc, string path) n_skill3 = ;
    // (string skillTitle, string desc, string path) n_skill4 = ;
    public (string skillTitle, string desc, string path)[] celestiaSkills = {
        (skillTitle: "Frost Strike", desc: "A attack that deals damage and slows the enemy.", path: "ChooseCharacters/skills/celestia/Skill (1)"),
        (skillTitle: "Frostbite", desc: "A spell that deals damage and freezes the enemy in front of her.", path: "ChooseCharacters/skills/celestia/Skill (2)"),
        (skillTitle: "Arcane", desc: "Freezes the enemy around her for 3s and 2s slow on movement speed after.", path: "ChooseCharacters/skills/celestia/Skill (3)"),
        (skillTitle: "Ice Ball", desc: "Spell a large Ice ball on the field that deals damage and slow them and gain 120% armor for 20s.", path: "ChooseCharacters/skills/celestia/Skill (4)")
    };
    public (string name, string desc) celestia = (name: "celestia", desc: "celestia  is a skilled mage fighter.");
}
