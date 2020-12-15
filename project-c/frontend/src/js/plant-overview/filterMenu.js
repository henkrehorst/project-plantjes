//check filter menu exist on page
if (document.getElementsByClassName('filter-checkbox').length > 0) {
    //store selected checkboxes
    let filterState = parseSearchParams();
    //fill search field
    fillSearch();

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

                removePaging();
                refreshPlants(false);
            }
        );
    });

    //connect search function with search button
    if (document.getElementById("search-button")) {
        document.getElementById("search-button").addEventListener('click', useSearchField)
    }

    //run search function on enter
    if (document.getElementById("search-field")) {
        document.getElementById("search-field").addEventListener("keyup", (e) => {
            if (e.key === "Enter") {
                e.preventDefault();
                useSearchField();
            }
        });
        //use search on clear
        document.getElementById("search-field").addEventListener("search", () => {
            if (document.getElementById("search-field").value === "") {
                useSearchField();
            }
        });
    }


    function useSearchField() {
        //get search value
        const searchVal = document.getElementById("search-field").value;

        if (searchVal.trim().length > 0) {
            filterState['naam'] = {searchVal: searchVal};
        } else if (Object.keys(filterState['naam']).length > 0) {
            delete filterState['naam'];
        }

        removePaging();
        refreshPlants(false)
    }

    function fillSearch() {
        if (filterState['naam'] !== undefined) {
            document.getElementById("search-field").value = filterState['naam'][Object.keys(filterState["naam"])[0]];
        } else {
            document.getElementById("search-field").value = "";
        }
    }

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
            const responseData = await response.json();
            //set correct current page in filter state
            let currentPage = responseData["pageIndex"];
            filterState["Page"] = {currentPage: currentPage};

            if (responseData["items"].length === 0) {
                document.getElementById('plantOverview').innerHTML = "<div class=\"overlay\" id=\"overlay\">\n" +
                    "                <div class=\"loader\"></div>\n" +
                    "            </div><h1>Geen planten gevonden</h1>";
            } else {
                let plantOverview = "<div class=\"overlay\" id=\"overlay\">\n" +
                    "                <div class=\"loader\"></div>\n" +
                    "            </div><h2 class=\"col-span-3 text-2xl font-semibold text-green-500\">" + responseData["count"] + " Stekjes</h2>";
                Object.entries(responseData["items"]).forEach(([key, plant]) => {
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
                
                //generate pagination
                if (responseData["hasMultiplePages"]) {
                    plantOverview += `<div class="col-span-3">
                        <div class="pt-4 flex justify-center">
                            <div class="relative z-0 inline-flex shadow-sm -space-x-px" aria-label="Pagination">
                                ${responseData["hasPreviousPage"] ?
                                `<button onclick="navigateToPage(${responseData['pageIndex'] - 1})" class="relative inline-flex items-center px-2 py-2 hover:text-black border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50">
                                        <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                                            <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd"/>
                                        </svg>
                                        Vorige
                                    </button>` : ""}
                               ${responseData["leftPagingNumbers"].map((i) => (
                                   `<button onclick="navigateToPage(${i})" class="relative inline-flex items-center px-4 py-2 hover:text-black border border-gray-300 text-sm font-medium ${i === responseData["pageIndex"] ? "bg-green-500 text-white" : "bg-white text-gray-700"} hover:bg-gray-50">
                                        ${i}
                                   </button>`
                                )).join("")}
                                ${responseData["hiddenPages"] && responseData["leftPagingNumbers"][responseData["leftPagingNumbers"].length - 1] + 1 !== (responseData["rightPagingNumbers"][0] !== undefined ? responseData["rightPagingNumbers"][0] : 0) ?
                                `<span class="relative inline-flex items-center px-4 py-2 hover:text-black border border-gray-300 bg-white text-sm font-medium text-gray-700">
                                        ...
                                    </span>` : ""}
                                ${responseData["rightPagingNumbers"].map((i) => (
                                    `<button onclick="navigateToPage(${i})" class="relative inline-flex items-center px-4 py-2 hover:text-black border border-gray-300 text-sm font-medium ${i === responseData["pageIndex"] ? "bg-green-500 text-white" : "bg-white text-gray-700"} hover:bg-gray-50">
                                        ${i}
                                    </button>`
                                )).join("")}
                                ${responseData["hasNextPage"] ?
                                `<button onclick="navigateToPage(${responseData['pageIndex'] + 1})"  class="relative inline-flex items-center hover:text-black px-2 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50">
                                        Volgende
                                        <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                                            <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"/>
                                        </svg>
                                    </button>` : ""}
                            </div>
                        </div>
                    </div>`;
                }

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

    window.navigateToPage = async function navigateToPage(page) {
        filterState["Page"] = {page: page};
        await refreshPlants(false);
        //scroll to highste row of plants
        window.scrollTo(0,426);
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

    function removePaging() {
        if (filterState["Page"] !== undefined) {
            delete filterState["Page"];
        }
    }

    //check user press back button or forward button
    window.onpopstate = async () => {
        //refresh filter state
        filterState = parseSearchParams();
        //select checkboxes from filter
        makeSelectedCheckboxes();
        //add correct content in search
        fillSearch();
        //refresh content
        await refreshPlants(true);
    };
}