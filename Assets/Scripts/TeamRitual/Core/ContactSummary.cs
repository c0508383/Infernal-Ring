using System.Collections.Generic;
using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Character;

namespace TeamRitual.Core {
public class ContactSummary {
    public CharacterStateMachine character;
    public List<ContactData> bodyColData;
    public List<ContactData> hurtColData;
    public List<ContactData> guardColData;
    public List<ContactData> armorColData;
    public List<ContactData> grabColData;
    public List<ContactData> techColData;

    public ContactSummary(CharacterStateMachine character) {
        this.bodyColData = new List<ContactData>();
        this.hurtColData = new List<ContactData>();
        this.guardColData = new List<ContactData>();
        this.armorColData = new List<ContactData>();
        this.grabColData = new List<ContactData>();
        this.techColData = new List<ContactData>();
    }

    public void SetData(List<ContactData> bodyColData, List<ContactData> hurtColData, List<ContactData> guardColData,
                        List<ContactData> armorColData, List<ContactData> grabColData, List<ContactData> techColData) {
        this.bodyColData = new List<ContactData>(bodyColData);
        this.hurtColData = new List<ContactData>(hurtColData);
        this.guardColData = new List<ContactData>(guardColData);
        this.armorColData = new List<ContactData>(armorColData);
        this.grabColData = new List<ContactData>(grabColData);
        this.techColData = new List<ContactData>(techColData);
    }

    public bool NotEmpty() {
        return bodyColData.Count > 0 || hurtColData.Count > 0 || guardColData.Count > 0 || armorColData.Count > 0 || grabColData.Count > 0 || techColData.Count > 0;
    }

    public void Clear() {
        bodyColData.Clear();
        hurtColData.Clear();
        guardColData.Clear();
        armorColData.Clear();
        grabColData.Clear();
        techColData.Clear();
    }
}
}