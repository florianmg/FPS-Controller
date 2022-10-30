# FPS Character Controller

Un character controller modulable en vue première personne qui utilise le composant `Character Controller` d'Unity3D comme base.

## Fonctionnalités

Chacune des fonctionnalités est activable/désactivable depuis l'inspecteur de composants

- Courrir
- Sauter
- S'accroupir
- Headbob
- Glisse sur les pentes
- Zoom
- Interactions
- Bruits de pas

## Interagir avec l'environnement

Pour créer un objet avec lequel le character controller peut interagir, il faut lui assigner un script étendu de la classe abstraite `Interactable`, ce qui lui donnera accès a 3 méthodes.

Exemple d'un script "Cube" :

    public class Cube : Interactable
    {
        public override void OnFocus()
        {
            print("Focus on " + gameObject.name);
        }

        public override void OnInteract()
        {
            print("Interact on " + gameObject.name);
        }

        public override void OnLoseFocus()
        {
            print("Lose focus from " + gameObject.name);
        }
    }

## Personnaliser les bruits de pas

Pour que les bruits de pas fonctionnent, il faut qu'il soient activés dans les options fonctionnels et avoie ajouté des clips dans les paramètres de bruits de pas.
Pour fonctionner, les bruits de pas utilisent le système de Tags d'Unity3D.
Par défaut, 3 bruits de pas sont présents, (Bois, Herbe, Default)

Pour ajouter des bruits de pas:

- Créer un tag correspondant au nouveau son dans Unity3D `Footsteps/AudioName`
- Ajouter une variable dans la partie **Footstep Parameters** `[SerializeField] private AudioClip[] audioNameClips = default;`
- Se rendre dans le script `FirstPersonController` dans la fonction `HandleFootsteps()` et ajouter un case dans le switch

### Layers

    - 9: Interactable

### Tags

    - Footsteps/Grass
    - Footsteps/Grass
