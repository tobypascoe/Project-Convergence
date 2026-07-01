[System.Serializable]
public struct StatChange
{
    public float health;
    public float hunger;
    public float thirst;

    public StatChange(float health, float hunger, float thirst)
    {
        this.health = health;
        this.hunger = hunger;
        this.thirst = thirst;
    }
}