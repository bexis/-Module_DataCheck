<script>
	import {countPlots} from './services/Caller';
	import { setApiConfig }  from '@bexis2/svelte-bexis2-core-ui';
	import { onMount,  } from 'svelte';

	let files;
	let plotsid = [];
	let result;
	let fileUpload = false;
	let header = false;

	onMount(async () => {
  		console.log("start edit");
  		//setApiConfig("https://localhost:44345","","");
	})

	let textareaPlots ="";

	const handleSubmit = () => {

		readFile(files[0]);

	}

	async function count()
	{
		//send to bexis textareaPlots
		plotsid = textareaPlots.split(/[\r\n,\t\s;]+/);
		console.log(plotsid);
		const respone = await countPlots(plotsid, header);
		console.log(respone);
		result = respone;
	}

	function clearTextbox()
	{
		textareaPlots = "";
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
		textareaPlots = text

      };

	}
</script>


<main>

	<div class="boxOuter">

	<p class="dtm-para_green">Enter plot ids in the text field or upload file.
	</p>

	<div class="boxLeft"><b>Plots:</b><br><textarea  bind:value={textareaPlots}></textarea><br>
		<button class="bx-button small function" on:click={clearTextbox}>
			Clear textbox
		</button><br>

	<input type="checkbox" bind:checked={fileUpload}/> <b>Upload plots via file</b>
	</div>
	{#if fileUpload == true}
	<div class="boxLeft">
		<input type="checkbox" bind:checked={header}/> File header
		<form on:submit|preventDefault={handleSubmit}>
			<input type="file" bind:files><br>
			<input type="submit" value="Submit" />	
		</form>
		</div>	{/if}
	
	<hr class="dtm-para_green"/>
	<button class="bx-button small function" on:click={count}>
		Count
	</button>
	<p class="errors"></p>
	</div>

	{#if result}

	<div class="boxOuter" id="results">
	
	<p class="dtm-para_green">Result</p>
	<ul>
		<li><b>Number Of Plots</b></li>
	{#each result.PlotProfiling.PlotTypeCounters as item, i}
	
		<p class="resultList"><b>Number of {item.PlotType}:</b> {item.Number}</p>

	{/each}
	<li><b>Joint Experiment 2020</b>
		<p class="resultList"><b>JointExperimentForest: {#if result.PlotProfiling.JointExperimentForest == true}
			 yes
		{:else}
			 no
		{/if}
			
		</b></p>
		<p class="resultList"><b>JointExperimentGrld: {#if result.PlotProfiling.JointExperimentGrld == true}
			 yes
		{:else}
			 no
		{/if}
		</b></p>
	</li>
	
	{#if result.NotVaildPlotIds.length > 0}

		<li><b>Non valid plots</b></li>
		<p class="resultList" style="overflow-y: scroll; height:100px;">
		{#each result.NotVaildPlotIds as item, i}
	
		{item}<br>

		{/each}
		</p>
	
	{/if}
	</ul>
	</div>
	{/if}

	
	

</main>

<style>
	main {
		text-align: left;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	textarea { 
		width: 150px; height: 200px; 
	}

	.boxLeft{
	 
     padding: 20px;
     background: #FFF;
	 width: 40%;
	}

	.boxOuter {
     float: left;
	 width: 30%;
	 border-left: #388670 6px solid;
	 border-right: #388670 2px solid;
     border-bottom: #388670 2px solid;
     border-top: #388670 10px solid;
     margin-bottom: 1em;
	 margin-right: 1em;
}



.dtm-para_green {
    background-color: #bbddd9;
    font-size: 14px;
    padding: 0.5em;
	margin-block-start: 0;
	border: 0;
}

.resultList
{
	margin-left: 100px;
	margin-right: 100px;
}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
</style>