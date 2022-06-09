import {Api} from "@bexis2/svelte-bexis2-core-ui";

export const countPlots = async (plots, header) => {

  let data = {
    plots: plots,
    header: header};

    try {
      const response = await Api.post('/DPT_BE/PlotProfiling/CountPlots', data);
      return response.data;
    } catch (error) {
      console.error(error);
    }
};