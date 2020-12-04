//check filter menu exist on page
if (document.getElementsByClassName('filter-checkbox').length > 0) {
    //store selected checkboxes
    let checkboxState = {};

    Array.from(document.getElementsByClassName('filter-checkbox')).forEach(function (element) {
        element.addEventListener('click', (event) => {
                //check filter category exists in state
                if (checkboxState[event.target.getAttribute('data-category')] === undefined) {
                    checkboxState[event.target.getAttribute('data-category')] = {};
                }

                //check set option id by filter in state if not exists
                if (checkboxState[event.target.getAttribute('data-category')][event.target.value] === undefined) {
                    checkboxState[event.target.getAttribute('data-category')][event.target.value] = event.target.value;
                } else {
                    //delete options in state disable checkbox
                    delete checkboxState[event.target.getAttribute('data-category')][event.target.value];
                }

                //delete filter category when no items exists
                if (Object.keys(checkboxState[event.target.getAttribute('data-category')]).length === 0) {
                    delete checkboxState[event.target.getAttribute('data-category')];
                }
                
                refreshPlants();
            }
        );
    });

    async function refreshPlants() {
        let searchUlr = new URLSearchParams();

        //build search url
        Object.entries(checkboxState).forEach(([category, options]) => {
            Object.entries(options).forEach(([k, v]) => {
                searchUlr.append(category, v);
            });
        });
        
        //request plants form backend
        await fetch(window.location.origin + '/api/plants?' + searchUlr.toString()).then(async response => {
            const plants = await response.json();

            if (plants.length === 0) {
                document.getElementById('plantOverview').innerHTML = "<h1>Geen planten gevonden</h1>";
            } else {
                let plantOverview = "";
                Object.entries(plants).forEach(([key, plant]) => {
                    plantOverview +=
                        `<div class="max-w-sm rounded overflow-hidden shadow-lg">
                            <a href="/plants/details/${plant.plantId}">
                                <img class="w-full plant-image" src="${plant.imgUrl}" alt="${plant.name}">
                                <div class="px-6 py-4">
                                    <div class="font-bold text-xl mb-2">${plant.name}</div>
                                </div>
                            </a>
                        </div>`
                });
                document.getElementById('plantOverview').innerHTML = plantOverview;
            }

        }).catch(err => {
            console.log(err)
        });
    }
}