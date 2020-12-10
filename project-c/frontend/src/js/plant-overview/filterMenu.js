//check filter menu exist on page
if (document.getElementsByClassName('filter-checkbox').length > 0) {
    //store selected checkboxes
    let filterState = parseSearchParams();

    Array.from(document.getElementsByClassName('filter-checkbox')).forEach(function (element) {
        //make checkboxes in url filter selected
        if (filterState[element.getAttribute('data-category')] !== undefined
            && filterState[element.getAttribute('data-category')][element.value.toString()] !== undefined) {
            element.checked = true;
        }

        element.addEventListener('click', (event) => {
                //check filter category exists in state
                if (filterState[event.target.getAttribute('data-category')] === undefined) {
                    filterState[event.target.getAttribute('data-category')] = {};
                }

                //check set option id by filter in state if not exists
                if (filterState[event.target.getAttribute('data-category')][event.target.value] === undefined) {
                    filterState[event.target.getAttribute('data-category')][event.target.value] = event.target.value;
                } else {
                    //delete options in state disable checkbox
                    delete filterState[event.target.getAttribute('data-category')][event.target.value];
                }

                //delete filter category when no items exists
                if (Object.keys(filterState[event.target.getAttribute('data-category')]).length === 0) {
                    delete filterState[event.target.getAttribute('data-category')];
                }

                refreshPlants(false);
            }
        );
    });

    async function refreshPlants(backReturn) {
        hideOrShowLoader();
        let searchUlr = new URLSearchParams();

        //build search url
        Object.entries(filterState).forEach(([category, options]) => {
            Object.entries(options).forEach(([k, v]) => {
                searchUlr.append(category, v);
            });
        });

        //request plants form backend
        await fetch(window.location.origin + '/api/plants?' + searchUlr.toString()).then(async response => {
            const plants = await response.json();

            if (plants.length === 0) {
                document.getElementById('plantOverview').innerHTML = "<div class=\"overlay\" id=\"overlay\">\n" +
                    "                <div class=\"loader\"></div>\n" +
                    "            </div><h1>Geen planten gevonden</h1>";
            } else {
                let plantOverview = "<div class=\"overlay\" id=\"overlay\">\n" +
                    "                <div class=\"loader\"></div>\n" +
                    "            </div>";
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
                hideOrShowLoader();
            }

        }).catch(err => {
            console.log(err)
        });

        //modify url
        if (backReturn === false) {
            if (history.pushState) {
                let url = window.location.origin + window.location.pathname + (searchUlr.toString().length > 0 ? "?" : "") + searchUlr.toString();
                window.history.pushState({path: url}, '', url);
            }
        }
    }

    function parseSearchParams() {
        const params = new URLSearchParams(window.location.search);
        let searchState = {};

        params.forEach((item, key) => {
            searchState[key] = {};
            params.getAll(key).forEach(value => {
                searchState[key][value] = value;
            });
        });

        return searchState;
    }

    function hideOrShowLoader() {
        document.getElementById("overlay").style.display =
            document.getElementById("overlay").style.display === "none" ? "initial" : "none";
    }

    function makeSelectedCheckboxes() {
        Array.from(document.getElementsByClassName('filter-checkbox')).forEach(function (element) {
            //make checkboxes in url filter selected
            element.checked = filterState[element.getAttribute('data-category')] !== undefined
                && filterState[element.getAttribute('data-category')][element.value.toString()] !== undefined;
        });
    }

    //check user press back button or forward button
    window.onpopstate = async () => {
        //refresh filter state
        filterState = parseSearchParams();
        //select checkboxes from filter
        makeSelectedCheckboxes();
        //refresh content
        await refreshPlants(true);
    };
}