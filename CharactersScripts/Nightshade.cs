using UnityEngine;

public class Nightshade
{
    public (string skillTitle, string desc, string path)[] nightshadeSkills = {
        (skillTitle: "Stab", desc: "Stab the enemy in front of him.", path: "ChooseCharacters/skills/nightshade/Skill (1)"),
        (skillTitle: "Stash", desc: "Spin and stash all of enemy around him on specific range.", path: "ChooseCharacters/skills/nightshade/Skill (2)"),
        (skillTitle: "Charge", desc: "Slows and deal damage to enemy in front of him.", path: "ChooseCharacters/skills/nightshade/Skill (3)"),
        (skillTitle: "Assassinate", desc: "Stun  all of the enemy in the field and gain 20% damage and  120% armor for 15 seconds.", path: "ChooseCharacters/skills/nightshade/Skill (4)"),
    };
    public (string name, string desc) nightshade = (name: "Nightshade", desc: "Nightshade  is a skilled ninja fighter, adept in both melee and ranged combat. He is strong and agile, able to swiftly close in on enemies with his melee weapon and engage from a distance. With his extensive training and physical prowess, he poses a formidable threat to any opponent.");
}
