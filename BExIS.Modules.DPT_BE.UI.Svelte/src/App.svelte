<script>
	
	//let container = document.getElementById("plotids");
	//let plotids = container.getAttribute("plots");

	import {countPlots} from './services/Caller';
	import { setApiConfig }  from '@bexis2/svelte-bexis2-core-ui';
	import { onMount,  } from 'svelte';

	let files;
	let plotsid = [];
	let result;

	onMount(async () => {
  		console.log("start edit");
  		
	})

	let textareaPlots = "";

	const handleSubmit = () => {

		readFile(files[0]);

	}

	async function count()
	{
		//send to bexis textareaPlots
		console.log("textareaPlots", textareaPlots);
		plotsid = textareaPlots.split('\r\n');
		const respone = await countPlots(plotsid);
		result = respone;
		console.log(respone);

	}

	function readFile(file)
	{
		const reader = new FileReader();
		const input = file;
		let json;

		reader.readAsText(input);

		reader.onload = function (e) {
		//debugger;	
        const text = e.target.result;
		console.log("text",text);
		textareaPlots = text;
        // const data = csvToArray(text);
		// JSON.stringify(data);

      };

	}

	// function csvToArray(str, delimiter = ",") 
	// {
	// 	const headers = str.slice(0, str.indexOf("\n")).split(delimiter);

	// 	let rows = str.slice(str.indexOf("\n") + 1).split("\n");
	// 	let array = rows.map(function (row) {
    // 	let values = row.split(delimiter);
    // 	let el = headers.reduce(function (object, header, index) {
    //   		object[header] = values[index];
    //   		return object;
    // 	}, {});
    // 		return el;
	// 	});

	// 	return array;
	// }

</script>


<main>
	<div class="boxLeft">
	Upload file or enter plot ids to the textfield<br><br>
	File Header: <input type="checkbox"/>

	<form on:submit|preventDefault={handleSubmit}>

		<div class="box"><input type="file" bind:files><br>

		<input type="submit" value="Submit" /></div>
	
	</form>
	</div>
	<div class="boxLeft"><textarea value={textareaPlots}></textarea>
		<br>
	<button on:click={count}>
		Count
	</button>
	</div>

	


	<div class="box" id="results">
	
	{#if result}
	<ul>
		<li><b>#Plots</b></li>
	{#each result.PlotProfiling.PlotTypeCounters as item, i}
	
		<p class="resultList">{item.PlotType}: {item.Number}</p>

	{/each}

	<li><b>#Not vaild plots:</b></li>
	<p class="resultList">
	{#each result.NotVaildPlotIds as item, i}
	
	{item}<br>

	{/each}
	</p>
	</ul>
	{:else}
		<b>...loading</b>
	{/if}

	</div>
	

</main>

<style>
	main {
		text-align: left;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	textarea { 
		width: 100px; height: 200px; 
	}

	.boxLeft {
     float: left;
     padding: 20px;
     background: #FFF;
	 width: 30%;
}

.box{
	width: 30%;
	
}

.resultList
{
	margin-left: 100px;
}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
</style>