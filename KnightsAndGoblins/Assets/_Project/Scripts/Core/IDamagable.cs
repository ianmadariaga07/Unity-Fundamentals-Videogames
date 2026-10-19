/// <summary>
/// Interfaz de contrato para cualquier entidad del juego que pueda recibir dano en combate.
/// Aplica polimorfismo y desacopla el sistema de ataque de las clases concretas de enemigos.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int damageAmount);
}