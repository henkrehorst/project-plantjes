//check filter menu exist on page
if (document.getElementsByClassName('filter-checkbox').length > 0) {
    //store selected checkboxes
    let filterState = parseSearchParams();
    //fill search field
    fillSearchZipcode();
    fillSortFilter();

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
        } else if (filterState['naam'] !== null | undefined && Object.keys(filterState['naam']).length > 0) {
            delete filterState['naam'];
        }

        removePaging();
        refreshPlants(false)
    }

    function fillSearchZipcode() {
        if (filterState['naam'] !== undefined) {
            document.getElementById("search-field").value = filterState['naam'][Object.keys(filterState["naam"])[0]];
        } else {
            document.getElementById("search-field").value = "";
        }

        if (filterState['postcode'] !== undefined) {
            document.getElementById("zipcode-field").value = filterState['postcode'][Object.keys(filterState["postcode"])[0]];
        } else {
            document.getElementById("zipcode-field").value = "";
        }

        if (filterState['Afstand'] !== undefined) {
            let optionCollection = document.getElementById("distance-select").options;

            for (let i = 0; i < optionCollection.length; i++) {
                if (filterState['Afstand'][Object.keys(filterState["Afstand"])[0]] === optionCollection[i].value) {
                    optionCollection[i].selected = 'selected';
                } else {
                    optionCollection[i].selected = '';
                }
            }
        }
    }

    function fillSortFilter() {
        if (filterState['Sort'] !== undefined) {
            let optionCollection = document.getElementById("sort-select").options;
            let optionCollectionMobile = document.getElementById("sort-select-mobile").options;

            for (let i = 0; i < optionCollection.length; i++) {
                if (filterState['Sort'][Object.keys(filterState["Sort"])[0]] === optionCollection[i].value) {
                    optionCollection[i].selected = 'selected';
                    if (filterState['Sort'][Object.keys(filterState["Sort"])[0]] === "loc")
                        document.getElementById("sort-filter-error").innerText = filterState["postcode"] === undefined ? "Geef uw postcode in om op afstand te filteren." : "";
                } else {
                    optionCollection[i].selected = '';
                }
            }

            for (let i = 0; i < optionCollectionMobile.length; i++) {
                if (filterState['Sort'][Object.keys(filterState["Sort"])[0]] === optionCollectionMobile[i].value) {
                    optionCollectionMobile[i].selected = 'selected';
                    if (filterState['Sort'][Object.keys(filterState["Sort"])[0]] === "loc")
                        document.getElementById("sort-filter-error-mobile").innerText = filterState["postcode"] === undefined ? "Geef uw postcode in om op afstand te filteren." : "";
                } else {
                    optionCollectionMobile[i].selected = '';
                }
            }
        }
    }

    //connect zipcode function with zipcode look button
    if (document.getElementById("zipcode-button")) {
        document.getElementById("zipcode-button").addEventListener('click', useZipcodeField)
    }

    //run zipcode function on enter
    if (document.getElementById("zipcode-field")) {
        document.getElementById("zipcode-field").addEventListener("keyup", (e) => {
            if (e.key === "Enter") {
                e.preventDefault();
                useZipcodeField();
            }
        });
        //use zipcode on clear
        document.getElementById("zipcode-field").addEventListener("search", () => {
            if (document.getElementById("zipcode-field").value === "") {
                useZipcodeField();
            }
        });
    }

    async function useZipcodeField() {
        //read zipcode from field
        const zipcode = document.getElementById("zipcode-field").value;

        if (zipcode.trim().length > 0) {
            fetch(window.location.origin + '/api/zipcode/' + zipcode).then(response => {
                if (!response.ok) {
                    document.getElementById('zipcode-field-error').innerText = "Ongeldige postcode!";
                } else {
                    document.getElementById('zipcode-field-error').innerText = "";
                    //get coordinates
                    response.json().then(data => {
                        let zipcode = data['zipCode'];
                        filterState['postcode'] = {zipcode: zipcode};
                        let lon = data['longitude'].toString();
                        filterState['lon'] = {lon: lon};
                        let lat = data['latitude'].toString();
                        filterState['lat'] = {lat: lat};

                        //remove distance error!
                        document.getElementById("afstand-field-error").innerText = "";

                        removePaging();
                        refreshPlants(false);
                    });
                }
            }).catch(error => {
                console.log(error);
            });
        } else {
            document.getElementById('zipcode-field-error').innerText = "";

            if (filterState['postcode'] !== undefined && Object.keys(filterState['postcode']).length > 0) {
                delete filterState['postcode'];
                delete filterState['lon'];
                delete filterState['lat'];
            }

            removePaging();
            refreshPlants(false);
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

        const searchUri = searchUlr.toString();

        //request plants form backend
        await fetch(window.location.origin + '/api/plants?' + searchUri).then(async response => {
            const responseData = await response.json();
            //set correct current page in filter state
            let currentPage = responseData["pageIndex"];
            filterState["Page"] = {currentPage: currentPage};

            if (responseData["items"].length === 0) {
                document.getElementById('plantOverview').innerHTML = "<div class=\"overlay\" id=\"overlay\">\n" +
                    "                <div class=\"loader\"></div>\n" +
                    "            </div><h1>Geen planten stekjes</h1>" +
                    `<div class="md:hidden">
                    <button onclick="openFilerMenu()" class="px-5 py-3 border border-transparent text-base font-medium rounded-md text-white bg-green-500 hover:bg-green-600 flex justify-center w-full">
                        <svg class="mr-2" style="fill: #ffffff" xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24">
                            <path d="M0 0h24v24H0z" fill="none"/><path d="M3 17v2h6v-2H3zM3 5v2h10V5H3zm10 16v-2h8v-2h-8v-2h-2v6h2zM7 9v2H3v2h4v2h2V9H7zm14 4v-2H11v2h10zm-6-4h2V7h4V5h-4V3h-2v6z"/>
                        </svg>Filter
                    </button>
                    </div>` +
                    document.getElementById("sort-filter-div").outerHTML;
                document.getElementById("result-button").innerText = "Geen stekjes gevonden";
            } else {
                let plantOverview = "<div class=\"overlay\" id=\"overlay\">\n" +
                    "                <div class=\"loader\"></div>\n" +
                    "            </div><h2 class=\"col-span-1 text-2xl font-semibold text-green-500 text-center sm:text-left\">" + responseData["count"] + " Stekjes</h2>" +
                    document.getElementById("sort-filter-div").outerHTML;
                plantOverview += `<div class="md:hidden">
                    <button onclick="openFilerMenu()" class="px-5 py-3 border border-transparent text-base font-medium rounded-md text-white bg-green-500 hover:bg-green-600 flex justify-center w-full">
                        <svg class="mr-2" style="fill: #ffffff" xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24">
                            <path d="M0 0h24v24H0z" fill="none"/><path d="M3 17v2h6v-2H3zM3 5v2h10V5H3zm10 16v-2h8v-2h-8v-2h-2v6h2zM7 9v2H3v2h4v2h2V9H7zm14 4v-2H11v2h10zm-6-4h2V7h4V5h-4V3h-2v6z"/>
                        </svg>Filter
                    </button>
                </div>`;
                Object.entries(responseData["items"]).forEach(([key, plant]) => {
                    if (plant.distance > 0) {
                        plantOverview +=
                            `<div class="rounded overflow-hidden shadow-lg relative">
                            <a href="/plants/details/${plant.plantId}">
                                <img class="w-full plant-image" src="${plant.imgUrl}" alt="${plant.name}">
                                <div class="absolute dis-label text-sm font-medium bg-green-100 py-1 px-2 rounded text-green-500 align-middle">${plant.distance} km</div>
                                <div class="px-6 py-4">
                                    <div class="font-bold text-xl mb-2">${plant.name}</div>
                                </div>
                            </a>
                        </div>`
                    } else {
                        plantOverview +=
                            `<div class="rounded overflow-hidden shadow-lg relative">
                            <a href="/plants/details/${plant.plantId}">
                                <img class="w-full plant-image" src="${plant.imgUrl}" alt="${plant.name}">
                                <div class="px-6 py-4">
                                    <div class="font-bold text-xl mb-2">${plant.name}</div>
                                </div>
                            </a>
                        </div>`
                    }
                });

                //generate pagination
                if (responseData["hasMultiplePages"]) {
                    plantOverview += `<div class="col-span-1 sm:col-span-2 lg:col-span-3 hidden sm:block">
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
                    </div>
                    <div class="col-span-1 sm:col-span-2 lg:col-span-3 sm:hidden">
                        <div class="pt-4 flex justify-center">
                            <div class="relative z-0 inline-flex shadow-sm -space-x-px" aria-label="Pagination">
                                ${responseData["hasPreviousPage"] ?
                        `<button onclick="navigateToPage(${responseData['pageIndex'] - 1})" class="relative inline-flex items-center px-2 py-2 hover:text-black border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50">
                                        <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                                            <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd"/>
                                        </svg>
                                        Vorige
                                    </button>` : ""}
                               ${responseData["mobilePagingNumbers"].map((i) => (
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

                    //update result button for mobile
                    document.getElementById("result-button").innerText = `Toon ${responseData['count']} stekjes`
                }

                document.getElementById('plantOverview').innerHTML = plantOverview;
                hideOrShowLoader();
                fillSortFilter();
            }

        }).catch(err => {
            console.log(err)
        });

        //modify url
        if (backReturn === false) {
            if (history.pushState) {
                let url = window.location.origin + window.location.pathname + (searchUri.length > 0 ? "?" : "") + searchUri;
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

    window.sortFilter = (option) => {
        let value = option.value;
        filterState["Sort"] = {value: value}

        if (option === "loc") {
            document.getElementById("sort-filter-error").innerText = filterState["postcode"] === undefined ? "Geef uw postcode in om op afstand te filteren." : "";
            document.getElementById("sort-filter-error-mobile").innerText = filterState["postcode"] === undefined ? "Geef uw postcode in om op afstand te filteren." : "";
        }else {
            document.getElementById("sort-filter-error").innerText = "";
            document.getElementById("sort-filter-error-mobile").innerText = "";
        }
        removePaging();
        refreshPlants(false);
    };


    window.distanceFilter = (option) => {
        let value = option.value;

        if (parseInt(value) > 0) {
            filterState["Afstand"] = {value: value}
        } else if (filterState["Afstand"] !== undefined) delete filterState["Afstand"];

        document.getElementById("afstand-field-error").innerText = filterState["postcode"] === undefined ? "Geef uw postcode in om op afstand te filteren." : "";
        removePaging();
        refreshPlants(false);
    }

    window.navigateToPage = async function navigateToPage(page) {
        filterState["Page"] = {page: page};
        await refreshPlants(false);
        //scroll to highste row of plants
        window.scrollTo(0, document.getElementById("plantOverview").offsetTop - 10);
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
        fillSearchZipcode();
        fillSortFilter();
        //refresh content
        await refreshPlants(true);
    };

    //listen on scroll
    document.addEventListener('scroll', (e) => {
        if (window.innerWidth < 790) {
            if (window.scrollY > 560 && !isFooterIntoView()) {
                document.getElementById("filter-button").style.display = "grid";
            } else {
                document.getElementById("filter-button").style.display = "none";
            }
        }else{
            document.getElementById("filter-button").style.display = "none";
        }
    });

    //check footer is visible
    function isFooterIntoView() {
        let rect = document.getElementById("footer").getBoundingClientRect();
        return (rect.top >= 0) && (rect.bottom - rect.height <= window.innerHeight);
    }
    
    window.openFilerMenu = () => {
            document.getElementById("filter-menu").classList.add("mobile-filter-open");
            document.getElementById("filter-result").classList.remove("hidden");
            document.getElementById("filter-result").classList.add("block");
            document.body.classList.add("--no-scroll");
    }

    window.closeFilerMenu = () => {
        document.getElementById("filter-menu").classList.remove("mobile-filter-open");
        document.getElementById("filter-result").classList.add("hidden");
        document.getElementById("filter-result").classList.remove("block");
        document.body.classList.remove("--no-scroll");
    }
    
    //when user press result button
    window.resultButton = () => {
        window.scrollTo(0, document.getElementById("plantOverview").offsetTop - 10);
        window.closeFilerMenu();
    }
    
    
}