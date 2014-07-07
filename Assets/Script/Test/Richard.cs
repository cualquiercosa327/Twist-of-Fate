﻿using UnityEngine;
using System.Collections;

public class Richard : MonoBehaviour
{
	public static Transform _Transform;

    PhysicsManager physManager;

	StateManager.State CurrentState
	{
		get { return physManager.State; }
	}

	public float TimerScatto = 0.5f;
	private float TimerScattoRimastoLeft = 0f;
    private float TimerScattoRimastoRight = 0f;
    private bool TastoDirezionalePrecedentementePremuto = false;
    private bool ScattoAttivato = false;
	public bool salto = false;
	public bool abbassato;
	public bool movimento;
	public bool scivo=false;
	public Color col;
    // Use this for initialization
    void Start()
    {
        physManager = GetComponent<PhysicsManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
	{
        // Mi memorizzo lo stato precedente per fare delle comparazioni
		StateManager.State state = CurrentState;
        switch (state)
        {
            case StateManager.State.Jumping:
            case StateManager.State.Falling:
            case StateManager.State.Scivolata:
                // Se in scivolata, disabilita ogni altro cambio di stato:
                // il motore della fisica funziona che, una volta che il
                // personaggio è in scivolata, ci resta finché l'inerzia
                // fa fermare la forza inizialmente assegnata dalla scivolata:
                // di conseguenza disabilita ogni altro tasto di input. Questo
                // per offrire un realismo maggiore.
                break;
            default:
                // Ottiene lo stato a partire dai tasti premuti
                state = getStateFromInput();

                // Controllo che evita di far saltare il personaggio mentre è abbassato
                if (CurrentState == StateManager.State.Crouch && state == StateManager.State.Jumping)
                {
                    state = CurrentState;
                }
                // Controllo che permette di fare la scivolata
			if (CurrentState == StateManager.State.Crouch && (We.Input.MoveLeft || We.Input.MoveRight) && !scivo)
			{
                    state = StateManager.State.PreScivolata;
				if (We.Input.MoveLeft )
					physManager.Direction = false;
				else
					physManager.Direction = true;
				scivo = true;
                }
                break;
        }
		physManager.State = state;
		_Transform = transform;

		if(!We.Input.MoveLeft && ! We.Input.MoveRight)
			scivo = false;
    }

	StateManager.State getStateFromInput()
	{
		//Controllo sul rilascio dei tasti
		if (!We.Input.Jump) {
			salto = false;	
		     }

        // TimerScatto serve per far scattare il personaggio alla doppia
        // pressione (velocememnte) di un tasto direzionale. Avremo due
        // contatori (uno per la freccia direzionale destra ed uno per la
        // sinistra) che aumenteranno all'infinito. Più avanti è spiegato
        // il perché viene fatta questa procedura.
		TimerScattoRimastoLeft += Time.deltaTime;
		TimerScattoRimastoRight += Time.deltaTime;

        // Il salto è posizionato qui sopra perché il giocatore deve permettere al
        // personaggio di farlo saltare sia in camminata sia in corsa. Inoltre, se
        // lo scatto è attivato ed il personaggio salta in corsa, all'atterraggio ha
        // bisogno di continuare la corsa. Sarebbe stato stupido il contrario, indi.
		if (We.Input.Jump == true && !salto && physManager.Stamina >= physManager.ConsumoStaminaSalto)
		{
			salto = true;
			return StateManager.State.Jumping;
		}
		if (We.Input.MoveLeft == true && !abbassato )
		{
            // Cambia direzione del personaggio in base al tasto direzionale premuto
			physManager.Direction = false;

            // Se il tasto corrente (che è stato appena premuto) non è stato premuto nel frame precedente
            // ed il tempo che è passato dalla prima volta che è stato premuto è minore del tempo richiesto
            // della doppia pressione per attivare lo scatto, allora vai e permetti lo scatto!
            if (((TastoDirezionalePrecedentementePremuto == false && TimerScattoRimastoLeft < TimerScatto)
                // Ritorna Run invece che Walk anche se lo scatto è comunque attivo.
			     || ScattoAttivato == true)&&physManager.Stamina > 0 )
            {
                // Dice che è in scatto.
                ScattoAttivato = true;
                return StateManager.State.Run;
            }
            else
            {
                // Vede se il tasto direzionale è stato precedentemente premuto
                if (TastoDirezionalePrecedentementePremuto == false)
                {
                    // Se no, ora dice il contrario
                    TastoDirezionalePrecedentementePremuto = true;
                    // Ma se ne approfitta per resettare il tempo utile per lo scatto
                    TimerScattoRimastoLeft = 0;
                }
                return StateManager.State.Walk;
            }
		}
		if (We.Input.MoveRight == true && !abbassato)
        {
            // Cambia direzione del personaggio in base al tasto direzionale premuto
			physManager.Direction = true;

            if (((TastoDirezionalePrecedentementePremuto == false && TimerScattoRimastoRight < TimerScatto)
			     || ScattoAttivato == true)&&physManager.Stamina > 0)
            {
                ScattoAttivato = true;
                return StateManager.State.Run;
            }
            else
            {
                if (TastoDirezionalePrecedentementePremuto == false)
                {
                    TastoDirezionalePrecedentementePremuto = true;
                    TimerScattoRimastoRight = 0;
                }
                return StateManager.State.Walk;
            }
		}
        // Qui dice che non è stato premuto alcun tasto direzionale
		TastoDirezionalePrecedentementePremuto = false;
        // e che lo scatto per la corsa è disattivato
		ScattoAttivato = false;

		if (We.Input.MoveDown == true && ! movimento)
        {
            // Abbassa il personaggio
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.10f);

			return StateManager.State.Crouch;
		}
		if (We.Input.Attack2 == true)
		{
			return StateManager.State.Attack;
		}

		return StateManager.State.Unpressed;
	}
}
