<script>
	
	//let container = document.getElementById("plotids");
	//let plotids = container.getAttribute("plots");

	import {countPlots} from './services/Caller';
	import { setApiConfig }  from '@bexis2/svelte-bexis2-core-ui';
	import { onMount,  } from 'svelte';



	let files;
	let plotsid = [];
	let json;

	onMount(async () => {
  		console.log("start edit");
  		setApiConfig("https://localhost:44345","epetzold","2021.B2.Go$On");
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
	Upload file or enter plot ids to the textfield<br><br>
	<form on:submit|preventDefault={handleSubmit}>

		<div class="box"><input type="file" bind:files><br>

		<input type="submit" value="Submit" /></div>
	
	</form>
	<div class="box"><textarea value={textareaPlots}></textarea></div>
	<br>
	<button on:click={count}>
		Count
	</button>
	


</main>

<style>
	main {
		text-align: left;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	/* h1 {
		color: #ff3e00;
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	} */
	textarea { 
		width: 100px; height: 200px; 
	}

	.box {
     float: left;
     padding: 20px;
     background: #eee;
}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
</style>